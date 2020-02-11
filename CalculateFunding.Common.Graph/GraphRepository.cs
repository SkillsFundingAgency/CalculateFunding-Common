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

        public GraphRepository(GraphDbSettings graphDbSettings)
        {
            Guard.ArgumentNotNull(graphDbSettings, nameof(graphDbSettings));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Url, nameof(graphDbSettings.Url));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Username, nameof(graphDbSettings.Username));
            Guard.IsNullOrWhiteSpace(graphDbSettings.Password, nameof(graphDbSettings.Password));

            IAuthToken authtoken = AuthTokens.Basic(graphDbSettings.Username, graphDbSettings.Password);

            _driver = GraphDatabase.Driver(graphDbSettings.Url, authtoken);
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
                    foreach (var query in indices.Select(_ => $"CREATE INDEX ON :{objectName}({_})"))
                    {
                        await session.RunAsync(query);
                    }
                }

                await session.WriteTransactionAsync(tx => CreateNodes(tx, nodes, key));
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
                await session.WriteTransactionAsync(tx => RemoveNode<T>(tx, field, value));
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private static async Task RemoveNode<T>(IAsyncTransaction tx, string field, string value)
        {
            string objectName = typeof(T).Name.ToLowerInvariant();
            string cypher = new StringBuilder()
                .AppendLine($"MATCH ({objectName[0]}:{objectName}{{{field}:'{value}'}})")
                .AppendLine($"DETACH DELETE {objectName[0]}")
                .ToString();

            await tx.RunAsync(cypher);
        }

        private static async Task CreateNodes<T>(IAsyncTransaction tx, IList<T> nodes, string key)
        {
            string objectName = typeof(T).Name.ToLowerInvariant();

            string cypher = new StringBuilder()
                .AppendLine($"UNWIND {{nodes}} AS {objectName}")
                .AppendLine($"MERGE ({objectName[0]}:{objectName}{(key != null ? key : string.Empty)})")
                .AppendLine($"SET {objectName[0]} = {objectName}")
                .ToString();

            await tx.RunAsync(cypher, new Dictionary<string, object>() { { "nodes", ParameterSerializer.ToDictionary(nodes) } });
        }


        public void Dispose()
        {
            _driver?.Dispose();
        }
    }
}
