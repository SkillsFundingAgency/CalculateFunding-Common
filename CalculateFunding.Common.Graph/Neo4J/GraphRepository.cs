using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Graph.Serializer;
using CalculateFunding.Common.Utility;
using Neo4j.Driver;
using GraphInterfaces = CalculateFunding.Common.Graph.Interfaces;

namespace CalculateFunding.Common.Graph.Neo4J
{
    public class GraphRepository : GraphInterfaces.IGraphRepository, IDisposable
    {
        private readonly IDriver _driver;
        private readonly ICypherBuilderFactory _cypherBuilderFactory;

        public GraphRepository(GraphDbSettings graphDbSettings, ICypherBuilderFactory cypherBuilderFactory, IDriver driver = null)
        {
            Guard.ArgumentNotNull(graphDbSettings, nameof(graphDbSettings));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Url, nameof(graphDbSettings.Url));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Username, nameof(graphDbSettings.Username));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Password, nameof(graphDbSettings.Password));
            Guard.ArgumentNotNull(cypherBuilderFactory, nameof(cypherBuilderFactory));

            IAuthToken authToken = AuthTokens.Basic(graphDbSettings.Username, graphDbSettings.Password);

            _driver = driver ?? GraphDatabase.Driver(graphDbSettings.Url, authToken);
            _cypherBuilderFactory = cypherBuilderFactory;
        }

        private IAsyncSession AsyncSession() => _driver.AsyncSession();
        public Task<IEnumerable<Entity<TNode>>> GetCircularDependencies<TNode>(string relationship, IEnumerable<IField> fields) where TNode : class => throw new NotImplementedException();

        public async Task<IEnumerable<Entity<TNode>>> GetCircularDependencies<TNode>(string relationship, GraphInterfaces.IField field)
            where TNode : class
        {
            IAsyncSession session = AsyncSession();

            try
            {
                string cypher = GetCircularDependencyCypher<TNode>(relationship, field);
                List<IRecord> records = await session.ReadTransactionAsync(tx => RunCypherWithResults(tx, cypher));
                return records?.Select(_ => new Entity<TNode> { Node = _[0].As<INode>().Properties.AsJson().AsPoco<TNode>(), Relationships = Transform(_[1].As<IPath>()) });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<IEnumerable<Entity<TNode>>> GetAllEntities<TNode>(GraphInterfaces.IField field, IEnumerable<string> relationships)
            where TNode:class
        {
            IAsyncSession session = AsyncSession();

            try
            {
                string cypher = GetAllCypher<TNode>(field, relationships);
                List<IRecord> records = await session.ReadTransactionAsync(tx => RunCypherWithResults(tx, cypher));
                return records?.Select(_ => new Entity<TNode> { Node = _[0].As<INode>().Properties.AsJson().AsPoco<TNode>(), Relationships = Transform(_[1].As<IPath>()) });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private IEnumerable<GraphInterfaces.IRelationship> Transform(IPath path)
        {
            IDictionary<long, INode> nodes = path.Nodes.Distinct().ToDictionary(_ => _.Id);

            return path.Relationships.Select(_ => new Relationship { One = nodes[_.StartNodeId].Properties, Two = nodes[_.EndNodeId].Properties, Type = _.Type });
        }

        public async Task UpsertNodes<T>(IEnumerable<T> nodes, IEnumerable<string> indices = null)
        {
            IAsyncSession session = AsyncSession();

            string objectName = typeof(T).Name.ToLowerInvariant();
            string key = null;

            try
            {
                if (!indices.IsNullOrEmpty())
                {
                    key = indices.Select(_ => $"{{{_}:{objectName}.{_}}}").FirstOrDefault();
                    foreach (var query in indices.Select(_ => $"INDEX ON :{objectName}({_})"))
                    {
                        ICypherBuilder cypherBuilder = _cypherBuilderFactory.NewCypherBuilder().AddCreate(query);
                        await session.RunAsync(cypherBuilder.ToString());
                    }
                }

                string cypher = UpsertNodesCypher<T>(key);
                await session.WriteTransactionAsync(tx => RunCypher(tx, cypher, new Dictionary<string, object>() { { "nodes", nodes.ToDictionaries() } }));
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task DeleteNode<T>(GraphInterfaces.IField field)
        {
            string cypher = RemoveNodeCypher<T>(field.Name, field.Value);

            await ExecuteCypher(cypher);
        }

        public async Task UpsertRelationship<A, B>(string relationShipName, GraphInterfaces.IField left, GraphInterfaces.IField right)
        {
            string cypher = UpsertRelationshipCypher<A, B>(relationShipName, left, right);
            
            await ExecuteCypher(cypher);
        }

        public async Task DeleteRelationship<A, B>(string relationShipName, GraphInterfaces.IField left, GraphInterfaces.IField right)
        {
            string cypher = DeleteRelationshipCypher<A, B>(relationShipName, left, right);

            await ExecuteCypher(cypher);
        }

        public Task DeleteNodes<T>(params GraphInterfaces.IField[] fields) => throw new NotImplementedException();
        public Task UpsertRelationships<A, B>(params AmendRelationshipRequest[] amendRelationshipRequests) => throw new NotImplementedException();

        public Task DeleteRelationships<A, B>(params AmendRelationshipRequest[] amendRelationshipRequests) => throw new NotImplementedException();

        public Task<IEnumerable<Entity<TNode>>> GetAllEntitiesForAll<TNode>(IEnumerable<GraphInterfaces.IField> fields,
            IEnumerable<string> relationships) where TNode : class =>
            throw new NotImplementedException();

        private async Task ExecuteCypher(string cypher, Dictionary<string, object> parameters = null)
        {
            IAsyncSession session = AsyncSession();

            try
            {
                await session.WriteTransactionAsync(tx => RunCypher(tx, cypher, parameters));
            }
            catch (Neo4jException e)
            {
                throw new GraphRepositoryException(e.Message, e);
            }
            finally
            {
                await session.CloseAsync();
            }    
        }

        private string RemoveNodeAndChildrenCypher<T>(GraphInterfaces.IField field)
        {
            string nodeName = typeof(T).Name.ToLowerInvariant();

            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = $"(({nodeName[0]}:{nodeName}{{{field.Name}:'{field.Value}'}})-[*0..]->(x))" } })
                .AddDetachDelete("x")
                .ToString();
        }

        private string GetCircularDependencyCypher<TNode>(string relationShip, GraphInterfaces.IField field)
        {
            string nodeName = typeof(TNode).Name.ToLowerInvariant();

            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = $"(e:{nodeName})" } })
                .AddWhere($"SIZE((e)<-[:{relationShip}] - ()) <> 0")
                .AddAnd($"SIZE(()<-[:{relationShip}] - (e)) <> 0")
                .AddAnd($"e.{field.Name} = '{field.Value}'")
                .AddMatch(new[] { new MatchWithAlias { Alias = "path", Pattern = $"(e) <-[:{relationShip} *]-(e)" } })
                .AddReturn(new[] { "e, path" })
                .ToString();
        }

        private string GetAllCypher<TNode>(GraphInterfaces.IField field, IEnumerable<string> relationships)
            where TNode:class
        {
            string node = typeof(TNode).Name.ToLowerInvariant();
            return _cypherBuilderFactory.NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = $"(e:{node})" } })
                .AddWhere($"e.{field.Name} = '{field.Value}'")
                .AddMatch(new[] { new MatchWithAlias { Alias = "path", Pattern = $"(e) <-[{string.Join('|',relationships.Select(_ => $":{_}"))} *]-()" } })
                .AddReturn(new[] { "e, path" })
                .ToString();
        }

        private string RemoveNodeCypher<T>(string field, string value)
        {
            string objectName = typeof(T).Name.ToLowerInvariant();
            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = $"({objectName[0]}:{objectName}{{{field}:'{value}'}})" } })
                .AddDetachDelete($"{objectName[0]}")
                .ToString();
        }

        private string UpsertNodesCypher<T>(string key)
        {
            string objectName = typeof(T).Name.ToLowerInvariant();
            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddUnwind($"{{nodes}} AS {objectName}")
                .AddMerge($"{objectName[0]}:{objectName}{key ?? string.Empty}")
                .AddSet($"{objectName[0]} = { objectName}")
                .ToString();
        }

        private string UpsertRelationshipCypher<A, B>(string relationShipName, GraphInterfaces.IField left, GraphInterfaces.IField right)
        {
            string objectAName = typeof(A).Name.ToLowerInvariant();
            string objectBName = typeof(B).Name.ToLowerInvariant();

            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = $"(a: {objectAName}),(b: {objectBName})" } })
                .AddWhere($"a.{left.Name} = '{left.Value}' and b.{right.Name} = '{right.Value}'")
                .AddMerge($"(a) -[:{relationShipName}]->(b)")
                .ToString();
        }

        private string DeleteRelationshipCypher<A, B>(string relationShipName, GraphInterfaces.IField left, GraphInterfaces.IField right)
        {
            string objectAName = typeof(A).Name.ToLowerInvariant();
            string objectBName = typeof(B).Name.ToLowerInvariant();

            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = $"(a: {objectAName})-[r:{relationShipName}]->(b: {objectBName})" } })
                .AddWhere($"a.{left.Name} = '{left.Value}' and b.{right.Name} = '{right.Value}'")
                .AddDelete("r")
                .ToString();
        }

        private async Task RunCypher(IAsyncTransaction tx, string cypher, Dictionary<string, object> parameters = null)
        {
            if (parameters != null)
            {
                await tx.RunAsync(cypher, parameters);
            }
            else
            {
                await tx.RunAsync(cypher);
            }
        }

        private async Task<List<IRecord>> RunCypherWithResults(IAsyncTransaction tx, string cypher, Dictionary<string, object> parameters = null)
        {
            IResultCursor result;

            if (parameters != null)
            {
                result = await tx.RunAsync(cypher, parameters);
            }
            else
            {
                result = await tx.RunAsync(cypher);
            }

            return await result?.ToListAsync();
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
