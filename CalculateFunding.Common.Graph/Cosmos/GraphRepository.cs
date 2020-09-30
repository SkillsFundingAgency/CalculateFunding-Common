using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Graph.Serializer;
using CalculateFunding.Common.Utility;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;

namespace CalculateFunding.Common.Graph.Cosmos
{
    public class GraphRepository : IGraphRepository
    {
        private static readonly HashSet<string> ImmutableProperties = new HashSet<string>
        {
            "id",
            PartitionKey
        };

        private const string PartitionKey = "partitionkey";
        private const string FallBackPartitionKey = "FallBackPartitionKey";

        private readonly IGremlinClientFactory _clientFactory;
        private readonly IPathResultsTransform _pathResultsTransform;

        public GraphRepository(IGremlinClientFactory clientFactory,
            IPathResultsTransform pathResultsTransform)
        {
            Guard.ArgumentNotNull(pathResultsTransform, nameof(pathResultsTransform));
            Guard.ArgumentNotNull(clientFactory, nameof(clientFactory));

            _pathResultsTransform = pathResultsTransform;
            _clientFactory = clientFactory;
        }

        public async Task<IEnumerable<Entity<TNode>>> GetCircularDependencies<TNode>(string relationship,
            IField field) where TNode : class
        {
            string vertexLabel = GetVertexLabel<TNode>();
            
            string query = $"g.{VertexTraversal(vertexLabel, field)}.as('A')" +
                           $".repeat({OutEdgeTraversal(relationship)}.inV().simplePath()).emit(loops().is(gte(1)))" +
                           $".{OutEdgeTraversal(relationship)}.inV().where(eq('A')).path()" +
                           ".dedup().by(unfold().order().by(id).dedup().fold())";

            IEnumerable<Dictionary<string, object>> results = await ExecuteQuery(query);

            return _pathResultsTransform.TransformAll<TNode>(results,
                vertexLabel,
                field);
        }

        public async Task<IEnumerable<Entity<TNode>>> GetAllEntities<TNode>(IField field,
            IEnumerable<string> relationships) where TNode : class
        {
            string vertexLabel = GetVertexLabel<TNode>();

            string query = $"g.{VertexTraversal(vertexLabel, field)}" +
                           $".coalesce({BothEdgeTraversal(relationships.ToArray())}.otherV().path(), path())";

            IEnumerable<Dictionary<string, object>> results = await ExecuteQuery(query);

            return _pathResultsTransform.TransformMatches<TNode>(results,
                vertexLabel,
                field);
        }

        public async Task UpsertNodes<TNode>(IEnumerable<TNode> nodes,
            IEnumerable<string> indices)
        {
            Guard.IsNotEmpty(indices, nameof(indices));

            string vertexLabel = GetVertexLabel<TNode>();
            string property = GetGremlinName(indices.First());

            IEnumerable<Dictionary<string, object>> vertices = nodes.ToDictionaries();

            Guard.Ensure(vertices.First().ContainsKey(property),
                $"Unable to upsert {vertexLabel} as did not locate index property {property}");

            foreach (Dictionary<string, object> vertex in vertices)
            {
                string value = vertex[property].ToString();
                string query = $"g.{VertexTraversal(vertexLabel, property, value)}.fold()" +
                               $".coalesce(unfold(), addV('{vertexLabel}'){ImmutablePropertyProcessors(vertex)})" +
                               $".{MutablePropertyProcessors(vertex)}";

                await ExecuteQuery(query);
            }
        }

        public async Task DeleteNode<T>(IField field)
        {
            string vertexLabel = GetVertexLabel<T>();

            string query = $"g.{VertexTraversal(vertexLabel, field)}.drop()";

            await ExecuteQuery(query);
        }

        public async Task UpsertRelationship<A, B>(string relationShipName,
            IField left,
            IField right)
        {
            string vertexALabel = GetVertexLabel<A>();
            string vertexBLabel = GetVertexLabel<B>();
            string edgeLabel = GetEdgeLabel(relationShipName);

            string query = $"g.{VertexTraversal(vertexALabel, left)}.as('A')" +
                           $".{VertexTraversal(vertexBLabel, right)}" +
                           $".coalesce(__.inE('{edgeLabel}').where(outV().as('A'))," +
                           $"addE('{edgeLabel}').from('A'))";

            await ExecuteQuery(query);
        }

        public async Task DeleteRelationship<A, B>(string relationShipName,
            IField left,
            IField right)
        {
            string vertexALabel = GetVertexLabel<A>();
            string vertexBLabel = GetVertexLabel<B>();
            string edgeLabel = GetEdgeLabel(relationShipName);

            string query = $"g.{VertexTraversal(vertexALabel, left)}.bothE('{edgeLabel}')" +
                           $".where(otherV().hasLabel('{vertexBLabel}').has('{GetPropertyName(right.Name)}', '{right.Value}')).drop()";

            await ExecuteQuery(query);
        }

        private async Task<IEnumerable<Dictionary<string, object>>> ExecuteQuery(string query)
        {
            try
            {
                using IGremlinClient gremlinClient = NewGremlinClient();

                ResultSet<Dictionary<string, object>> response = await gremlinClient.SubmitAsync<Dictionary<string, object>>(query);

                return response;
            }
            catch (ResponseException e)
            {
                throw new GraphRepositoryException(e.Message, e);
            }
        }

        private static string VertexTraversal(string vertexLabel,
            string property,
            string value)
            => $"V().hasLabel('{vertexLabel}').has('{GetPropertyName(property)}', '{value}')";

        private static string VertexTraversal(string vertexLabel,
            IField identifier)
            => VertexTraversal(vertexLabel,
                identifier.Name,
                identifier.Value);

        private static string BothEdgeTraversal(params string[] edgeLabels)
            => $"bothE({GetEdgeLabels(edgeLabels)})";
        
        private static string OutEdgeTraversal(params string[] edgeLabels)
            => $"outE({GetEdgeLabels(edgeLabels)})";
        
        private static string InEdgeTraversal(params string[] edgeLabels)
            => $"inE({GetEdgeLabels(edgeLabels)})";

        private static string GetEdgeLabels(params string[] edgeLabels)
            => edgeLabels.Select(edgeLabel => $"'{GetEdgeLabel(edgeLabel)}'").JoinWith(',');

        private static string ImmutablePropertyProcessors(IDictionary<string, object> properties)
        {
            if (!properties.ContainsKey(PartitionKey))
            {
                properties[PartitionKey] = FallBackPartitionKey;
            }

            return $".{PropertyProcessors(properties.Where(IsImmutable))}";
        } 

        private static string MutablePropertyProcessors(IDictionary<string, object> properties)
            => PropertyProcessors(properties.Where(IsNotImmutable));

        private static bool IsImmutable(KeyValuePair<string, object> property) => ImmutableProperties.Contains(property.Key);
        
        private static bool IsNotImmutable(KeyValuePair<string, object> property) => !ImmutableProperties.Contains(property.Key);

        private static string PropertyProcessors(IEnumerable<KeyValuePair<string, object>> properties)
            => properties.Select(_ => $"property(single, '{GetPropertyName(_.Key)}', '{_.Value}')").JoinWith('.');

        private static string GetVertexLabel<TVertex>() => GetGremlinName(typeof(TVertex).Name);

        private static string GetEdgeLabel(string edgeLabel) => GetGremlinName(edgeLabel);

        private static string GetPropertyName(string propertyName) => GetGremlinName(propertyName);

        private static string GetGremlinName(string name) => name.ToLowerInvariant();

        private IGremlinClient NewGremlinClient() => _clientFactory.CreateClient();
    }
}