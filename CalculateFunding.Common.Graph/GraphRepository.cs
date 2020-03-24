using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Utility;
using CalculateFunding.Services.Graph.Serializer;
using CalculateFunding.Common.Extensions;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Graph
{
    public class GraphRepository : IGraphRepository, IDisposable
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

        public async Task<IEnumerable<Entity<TNode, TRelationship>>> GetCircularDependencies<TNode, TRelationship>(string relationShip, IField field) 
            where TNode : class 
            where TRelationship : class
        { 
            IAsyncSession session = AsyncSession();

            try
            {
                string cypher = GetCircularDependencyCypher(relationShip, field);
                List<IRecord> records = await session.ReadTransactionAsync(tx => RunCypherWithResults(tx, cypher));
                return records?.Select(_ => new Entity<TNode, TRelationship> { 
                    Node = _[0].As<INode>().Properties.AsJson().AsPoco<TNode>(), 
                    Relationship = _[1].AsJson().AsPoco<TRelationship>()
                });
            }
            finally
            {
                await session.CloseAsync();
            }
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
                await session.WriteTransactionAsync(tx => RunCypher(tx, cypher, new Dictionary<string, object>() { { "nodes", ParameterSerializer.ToDictionary(nodes) } }));
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task DeleteNodeAndChildNodes<T>(IField field)
        {
            string cypher = RemoveNodeAndChildrenCypher<T>(field);

            await ExecuteCypher(cypher);
        }

        public async Task DeleteNode<T>(IField field)
        {
            string cypher = RemoveNodeCypher<T>(field.Name, field.Value);

            await ExecuteCypher(cypher);
        }

        public async Task UpsertRelationship<A, B>(string relationShipName, IField left, IField right)
        {
            string cypher = UpsertRelationshipCypher<A, B>(relationShipName, left, right);
            
            await ExecuteCypher(cypher);
        }

        public async Task DeleteRelationship<A, B>(string relationShipName, IField left, IField right)
        {
            string cypher = DeleteRelationshipCypher<A, B>(relationShipName, left, right);

            await ExecuteCypher(cypher);
        }

        private async Task ExecuteCypher(string cypher, Dictionary<string, object> parameters = null)
        {
            IAsyncSession session = AsyncSession();

            try
            {
                await session.WriteTransactionAsync(tx => RunCypher(tx, cypher, parameters));
            }
            finally
            {
                await session.CloseAsync();
            }    
        }

        private string RemoveNodeAndChildrenCypher<T>(IField field)
        {
            string nodeName = typeof(T).Name.ToLowerInvariant();

            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = $"(({nodeName[0]}:{nodeName}{{{field.Name}:'{field.Value}'}})-[*0..]->(x))" } })
                .AddDetachDelete("x")
                .ToString();
        }

        private string GetCircularDependencyCypher(string relationShip, IField field)
        {
            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = "(e)" } })
                .AddWhere($"SIZE((e)<-[:{relationShip}] - ()) <> 0")
                .AddAnd($"SIZE(()<-[:{relationShip}] - (e)) <> 0")
                .AddAnd($"e.{field.Name} = '{field.Value}'")
                .AddMatch(new[] { new MatchWithAlias { Alias = "path", Pattern = $"(e) <-[:{relationShip} *]-(e)" } })
                .AddReturn(new[] { "e", "path" })
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

        private string UpsertRelationshipCypher<A, B>(string relationShipName, IField left, IField right)
        {
            string objectAName = typeof(A).Name.ToLowerInvariant();
            string objectBName = typeof(B).Name.ToLowerInvariant();

            return _cypherBuilderFactory
                .NewCypherBuilder()
                .AddMatch(new[] { new Match { Pattern = $"(a: {objectAName}),(b: {objectBName})" } })
                .AddWhere($"a.{left.Name} = '{left.Value}' and b.{right.Name} = '{right.Value}'")
                .AddCreate($"(a) -[:{relationShipName}]->(b)")
                .ToString();
        }

        private string DeleteRelationshipCypher<A, B>(string relationShipName, IField left, IField right)
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
