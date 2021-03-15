using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Graph.Serializer;
using CalculateFunding.Common.Utility;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using static CalculateFunding.Common.Graph.Threading.ParallelRunner;

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
        private readonly int _degreeOfParallelism;

        public GraphRepository(IGremlinClientFactory clientFactory,
            IPathResultsTransform pathResultsTransform,
            int degreeOfParallelism = 5)
        {
            Guard.ArgumentNotNull(pathResultsTransform, nameof(pathResultsTransform));
            Guard.ArgumentNotNull(clientFactory, nameof(clientFactory));

            _pathResultsTransform = pathResultsTransform;
            _clientFactory = clientFactory;
            _degreeOfParallelism = degreeOfParallelism;
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

        public async Task<IEnumerable<Entity<TNode>>> GetAllEntitiesForAll<TNode>(IEnumerable<IField> fields,
            IEnumerable<string> relationships) where TNode : class
        {
            using IGremlinClient gremlinClient = GremlinClient();
            return (await RunForAllItemsAndReturn(fields.ToArray(),
                    field => GetAllEntities<TNode>(field, relationships, gremlinClient),
                    _degreeOfParallelism))
                .SelectMany(_ => _);
        }

        public async Task<IEnumerable<Entity<TNode>>> GetAllEntities<TNode>(IField field,
            IEnumerable<string> relationships) where TNode : class
        {
            return await GetAllEntities<TNode>(field, relationships, null);
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

            using IGremlinClient gremlinClient = GremlinClient();
            await RunForAllItems(vertices.ToArray(),
                vertex =>
                {
                    string value = vertex[property].ToString();
                    string query = $"g.{VertexTraversal(vertexLabel, property, value)}.fold()" +
                                   $".coalesce(unfold(), addV('{vertexLabel}'){ImmutablePropertyProcessors(vertex)})" +
                                   $".{MutablePropertyProcessors(vertex)}";

                    return ExecuteQuery(query, gremlinClient);
                },
                _degreeOfParallelism);
        }

        public async Task DeleteNodes<T>(params IField[] fields)
        {
            Guard.IsNotEmpty(fields, nameof(fields));

            using IGremlinClient gremlinClient = GremlinClient();
            await RunForAllItems(fields, field => DeleteNode<T>(field, gremlinClient));
        }

        public async Task DeleteNode<T>(IField field)
        {
            await DeleteNode<T>(field, null);
        }

        public async Task UpsertRelationship<A, B>(string relationShipName,
            IField left,
            IField right)
        {
            await UpsertRelationship<A, B>(relationShipName,
                                            left,
                                            right,
                                            null);
        }

        public async Task UpsertRelationships<A, B>(params AmendRelationshipRequest[] amendRelationshipRequests)
        {
            using IGremlinClient gremlinClient = GremlinClient();
            await RunForAllItems(amendRelationshipRequests, 
                relationship => UpsertRelationship<A, B>(relationship.Type, relationship.A, relationship.B, gremlinClient));
        }

        public async Task DeleteRelationship<A, B>(string relationShipName,
            IField left,
            IField right)
        {
            await DeleteRelationship<A, B>(relationShipName,
                                            left,
                                            right,
                                            null);
        }
        
        public async Task DeleteRelationships<A, B>(params AmendRelationshipRequest[] amendRelationshipRequests)
        {
            using IGremlinClient gremlinClient = GremlinClient();
            await RunForAllItems(amendRelationshipRequests, 
                relationship => DeleteRelationship<A, B>(relationship.Type, relationship.A, relationship.B, gremlinClient));
        }

        private async Task DeleteNode<T>(IField field, IGremlinClient gremlinClient)
        {
            string vertexLabel = GetVertexLabel<T>();

            string query = $"g.{VertexTraversal(vertexLabel, field)}.drop()";

            await ExecuteQuery(query, gremlinClient);
        }

        private async Task UpsertRelationship<A, B>(string relationShipName,
            IField left,
            IField right,
            IGremlinClient graphClient)
        {
            string vertexALabel = GetVertexLabel<A>();
            string vertexBLabel = GetVertexLabel<B>();
            string edgeLabel = GetEdgeLabel(relationShipName);

            string query = $"g.{VertexTraversal(vertexALabel, left)}.as('A')" +
                           $".{VertexTraversal(vertexBLabel, right)}" +
                           $".coalesce(__.inE('{edgeLabel}').where(outV().as('A'))," +
                           $"addE('{edgeLabel}').from('A'))";

            await ExecuteQuery(query, graphClient);
        }

        private async Task DeleteRelationship<A, B>(string relationShipName,
            IField left,
            IField right,
            IGremlinClient gremlinClient)
        {
            string vertexALabel = GetVertexLabel<A>();
            string vertexBLabel = GetVertexLabel<B>();
            string edgeLabel = GetEdgeLabel(relationShipName);

            string query = $"g.{VertexTraversal(vertexALabel, left)}.bothE('{edgeLabel}')" +
                           $".where(otherV().hasLabel('{vertexBLabel}').has('{GetPropertyName(right.Name)}', '{GetPropertyValue(right.Value)}')).drop()";

            await ExecuteQuery(query, gremlinClient);
        }

        private async Task<IEnumerable<Entity<TNode>>> GetAllEntities<TNode>(IField field,
            IEnumerable<string> relationships, IGremlinClient gremlinClient = null) where TNode : class
        {
            string vertexLabel = GetVertexLabel<TNode>();

            string query = $"g.{VertexTraversal(vertexLabel, field)}" +
                           $".coalesce({BothEdgeTraversal(relationships.ToArray())}.otherV().path(), path())";

            IEnumerable<Dictionary<string, object>> results = await ExecuteQuery(query, gremlinClient);

            return _pathResultsTransform.TransformMatches<TNode>(results,
                vertexLabel,
                field);
        }

        private async Task<IEnumerable<Dictionary<string, object>>> ExecuteQuery(string query, IGremlinClient client = null)
        {
            try
            {
                if (client == null)
                {
                    using IGremlinClient graphclient = GremlinClient();
                    return await graphclient.SubmitAsync<Dictionary<string, object>>(query);
                }

                return await client.SubmitAsync<Dictionary<string, object>>(query);
            }
            catch (ResponseException e)
            {
                throw new GraphRepositoryException(e.Message, e);
            }
        }

        private static string VertexTraversal(string vertexLabel,
            string property,
            string value)
            => $"V().hasLabel('{vertexLabel}').has('{GetPropertyName(property)}', '{GetPropertyValue(value)}')";

        private static string VertexTraversal(string vertexLabel,
            IField identifier)
            => VertexTraversal(vertexLabel,
                identifier.Name,
                GetPropertyValue(identifier.Value));

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
            => properties.Select(_ => $"property(single, '{GetPropertyName(_.Key)}', '{GetPropertyValue(_.Value)}')").JoinWith('.');

        private static string GetVertexLabel<TVertex>() => GetGremlinName(typeof(TVertex).Name);

        private static string GetEdgeLabel(string edgeLabel) => GetGremlinName(edgeLabel);

        private static string GetPropertyName(string propertyName) => GetGremlinName(propertyName);

        private static string GetPropertyValue(object propertyValue) 
            => propertyValue?
                .ToString()
                .Replace("\'", "\\\'");

        private static string GetGremlinName(string name) => name.ToLowerInvariant();

        private IGremlinClient GremlinClient() => _clientFactory.Client;
    }
}