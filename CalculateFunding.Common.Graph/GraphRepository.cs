using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Utility;
using CalculateFunding.Services.Graph.Serializer;
using CalculateFunding.Common.Extensions;
using Neo4j.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Graph
{
    public class GraphRepository : IGraphRepository, IDisposable
    {
        private IDriver _driver;
        private ICypherBuilderHost _cypherBuilderHost;

        public GraphRepository(GraphDbSettings graphDbSettings, ICypherBuilderHost cypherBuilderHost, IDriver driver = null)
        {
            Guard.ArgumentNotNull(graphDbSettings, nameof(graphDbSettings));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Url, nameof(graphDbSettings.Url));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Username, nameof(graphDbSettings.Username));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Password, nameof(graphDbSettings.Password));
            Guard.ArgumentNotNull(cypherBuilderHost, nameof(cypherBuilderHost));

            IAuthToken authtoken = AuthTokens.Basic(graphDbSettings.Username, graphDbSettings.Password);

            _driver = driver ?? GraphDatabase.Driver(graphDbSettings.Url, authtoken);
            _cypherBuilderHost = cypherBuilderHost;
        }

        public async Task AddNodes<T>(IList<T> nodes, IEnumerable<string> indices = null)
        {
            IAsyncSession session = _driver.AsyncSession();

            string objectName = typeof(T).Name.ToLowerInvariant();
            string key = null;

            try
            {

                if (!indices.IsNullOrEmpty())
                {
                    key = indices.Select(_ => $"{{{_}:{objectName}.{_}}}").FirstOrDefault();
                    foreach (var query in indices.Select(_ => $"INDEX ON :{objectName}({_})"))
                    {
                        ICypherBuilder cypherBuilder = _cypherBuilderHost.Current().AddCreate(query);
                        await session.RunAsync(cypherBuilder.ToString());
                    }
                }

                string cypher = CreateNodesCypher<T>(key);
                await session.WriteTransactionAsync(tx => RunCypher(tx, cypher, new Dictionary<string, object>() { { "nodes", ParameterSerializer.ToDictionary(nodes) } }));
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task DeleteNode<T>(string field, string value)
        {
            IAsyncSession session = _driver.AsyncSession();

            try
            {
                string cypher = RemoveNodeCypher<T>(field, value);
                await session.WriteTransactionAsync(tx => RunCypher(tx, cypher));
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateRelationship<A, B>(string relationShipName, (string field, string value) left, (string field, string value) right)
        {
            IAsyncSession session = _driver.AsyncSession();

            try
            {
                string cypher = CreateRelationshipCypher<A, B>(relationShipName, left, right);
                await session.WriteTransactionAsync(tx => RunCypher(tx, cypher));
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private string RemoveNodeCypher<T>(string field, string value)
        {
            string objectName = typeof(T).Name.ToLowerInvariant();
            return _cypherBuilderHost
                .Current()
                .AddMatch($"({objectName[0]}:{objectName}{{{field}:'{value}'}})")
                .AddDetachDelete($"{objectName[0]}")
                .ToString();
        }

        private string CreateNodesCypher<T>(string key)
        {
            string objectName = typeof(T).Name.ToLowerInvariant();
            return _cypherBuilderHost
                .Current()
                .AddUnwind($"{{nodes}} AS {objectName}")
                .AddMerge($"{objectName[0]}:{objectName}{(key != null ? key : string.Empty)}")
                .AddSet($"{objectName[0]} = { objectName}")
                .ToString();
        }

        private string CreateRelationshipCypher<A, B>(string relationShipName, (string field, string value) left, (string field, string value) right)
        {
            string objectAName = typeof(A).Name.ToLowerInvariant();
            string objectBName = typeof(B).Name.ToLowerInvariant();

            return _cypherBuilderHost
                .Current()
                .AddMatch($"a: {objectAName}),(b: {objectBName}")
                .AddWhere($"a.{left.field} = '{left.value}' and b.{right.field} = '{right.value}'")
                .AddCreate($"(a) -[:{relationShipName}]->(b)")
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

        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
