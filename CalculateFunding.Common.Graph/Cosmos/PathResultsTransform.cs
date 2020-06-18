using System;
using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Graph.Serializer;

namespace CalculateFunding.Common.Graph.Cosmos
{
    public class PathResultsTransform : IPathResultsTransform
    {
        public IEnumerable<Entity<TNode>> TransformMatches<TNode>(IEnumerable<Dictionary<string, object>> results,
            string vertexLabel,
            IField identifier)
            where TNode : class
            => GetVertices<TNode>(results, vertexLabel, identifier).ToArray();

        public IEnumerable<Entity<TNode>> TransformAll<TNode>(IEnumerable<Dictionary<string, object>> results,
            string vertexLabel,
            IField identifier)
            where TNode : class
            => GetVertices<TNode>(results, vertexLabel, identifier, true).ToArray();

        private static IEnumerable<Entity<TNode>> GetVertices<TNode>(IEnumerable<Dictionary<string, object>> results,
            string vertexLabel,
            IField identifier,
            bool includeAllVertices = false)
            where TNode : class
        {
            Dictionary<string, Entity<TNode>> matchingVertices = new Dictionary<string, Entity<TNode>>();
            Dictionary<string, dynamic> allVertices = new Dictionary<string, dynamic>();
            Dictionary<string, Edge> edges = new Dictionary<string, Edge>();

            foreach (Dictionary<string, object> result in results)
            {
                (bool located, dynamic objects) = TryGetItem<dynamic>(result, "objects");

                if (!located)
                {
                    continue;
                }

                TransformObjects<TNode>(objects,
                    vertexLabel,
                    identifier,
                    includeAllVertices,
                    matchingVertices,
                    allVertices,
                    edges);
            }

            TransformRelationships(edges, matchingVertices, allVertices);

            return matchingVertices.Values;
        }

        private static void TransformObjects<TNode>(dynamic objects,
            string vertexLabel,
            IField identifier,
            bool includeAllVertices,
            Dictionary<string, Entity<TNode>> matchingVertices,
            Dictionary<string, dynamic> allVertices,
            Dictionary<string, Edge> edges)
            where TNode : class
        {
            foreach (dynamic @object in objects)
            {
                Dictionary<string, object> properties = @object as Dictionary<string, object>;

                if (properties == null)
                {
                    continue;
                }

                if (IsVertex(properties))
                {
                    (string id, Dictionary<string, object> properties) vertexProperties = GetVertexProperties(properties, allVertices);

                    if (includeAllVertices || IsMatchingEntity(properties,
                        vertexProperties.properties,
                        vertexLabel,
                        identifier))
                    {
                        CreateVertex(vertexProperties, matchingVertices);
                    }
                }

                if (IsEdge(properties))
                {
                    CreateEdge(properties, edges);
                }
            }
        }

        private static void CreateVertex<TNode>((string id, Dictionary<string, object> mergedProperties) vertexProperties,
            Dictionary<string, Entity<TNode>> matchingVertices) where TNode : class
        {
            Entity<TNode> entity = new Entity<TNode>
            {
                Node = vertexProperties.mergedProperties.AsJson().AsPoco<TNode>()
            };

            matchingVertices[vertexProperties.id] = entity;
        }

        private static (string id, Dictionary<string, object> properties) GetVertexProperties(Dictionary<string, object> properties,
            Dictionary<string, object> vertices)
        {
            (string id, Dictionary<string, object> mergedProperties) = GetMergedProperties(properties);

            vertices[id] = mergedProperties;

            return (id, mergedProperties);
        }

        private static void CreateEdge(Dictionary<string, object> properties,
            Dictionary<string, Edge> relationships)
        {
            string id = GetItem<string>(properties, "id");

            if (relationships.ContainsKey(id))
            {
                return;
            }
            
            string inV = GetItem<string>(properties, "inV");
            string outV = GetItem<string>(properties, "outV");

            Relationship relationship = new Relationship
            {
                Type = GetItem<string>(properties, "label")
            };

            relationships.Add(id, new Edge(outV, inV, relationship));
        }

        private static void TransformRelationships<TNode>(Dictionary<string, Edge> relationships,
            Dictionary<string, Entity<TNode>> matchingVertices,
            Dictionary<string, object> allVertices) where TNode : class
        {
            foreach (Edge edge in relationships.Values)
            {
                Relationship relationship = edge.Relationship;

                relationship.One = GetItem<Dictionary<string, object>>(allVertices, edge.outV);
                relationship.Two = GetItem<Dictionary<string, object>>(allVertices, edge.inV);

                if (!TryGetNode(matchingVertices, edge.outV, out Entity<TNode> matchingVertex))
                {
                    matchingVertex = GetNode(matchingVertices, edge.inV);
                }

                matchingVertex.AddRelationship(relationship);
            }
        }

        private static (string id, Dictionary<string, object> properties) GetMergedProperties(Dictionary<string, object> properties)
        {
            Dictionary<string, object> mergedProperties = GetItem<Dictionary<string, object>>(properties, "properties");

            OverwriteWithValues(mergedProperties);

            const string key = "id";

            string id = GetItem<string>(properties, key);

            mergedProperties.Add(key, id);

            return (id, mergedProperties);
        }

        private static void OverwriteWithValues(Dictionary<string, object> properties)
        {
            foreach (KeyValuePair<string, dynamic> property in properties.ToDictionary())
            foreach (dynamic value in property.Value)
            {
                properties[property.Key] = GetItem<string>((Dictionary<string, object>) value, nameof(value));
            }
        }

        private static (bool located, TCast item) TryGetItem<TCast>(Dictionary<string, object> properties,
            string key) =>
            (properties.TryGetValue(key, out dynamic property), (TCast) property);

        private static TCast GetItem<TCast>(Dictionary<string, object> properties,
            string key) =>
            properties.TryGetValue(key, out dynamic property) ? (TCast) property : throw new InvalidOperationException($"No item with key:'{key}' located in dictionary");

        private static bool TryGetNode<TNode>(Dictionary<string, Entity<TNode>> properties,
            string key,
            out Entity<TNode> match)
            where TNode : class =>
            properties.TryGetValue(key, out match); 
        
        private static Entity<TNode> GetNode<TNode>(Dictionary<string, Entity<TNode>> properties,
            string key)
            where TNode : class =>
            properties.TryGetValue(key, out Entity<TNode> property) ? property : throw new InvalidOperationException($"No matching vertex with id:'{key}' located in dictionary");

        private static bool IsMatchingEntity(Dictionary<string, object> properties,
            Dictionary<string, object> vertexProperties,
            string vertexLabel,
            IField identifier) =>
            HasKeyValuePair(properties, "label", vertexLabel) &&
            HasKeyValuePair(vertexProperties, identifier.Name.ToLowerInvariant(), identifier.Value);

        private static bool IsVertex(Dictionary<string, object> properties) =>
            HasKeyValuePair(properties, "type", "vertex");

        private static bool IsEdge(Dictionary<string, object> properties) =>
            HasKeyValuePair(properties, "type", "edge");

        private static bool HasKeyValuePair(IDictionary<string, object> properties,
            string key,
            string value)
            => properties.TryGetValue(key, out object actual) && actual?.Equals(value) == true;

        private readonly struct Edge
        {
            public Edge(string outV,
                string inV,
                Relationship relationship)
            {
                Relationship = relationship;
                this.outV = outV;
                this.inV = inV;
            }

            public Relationship Relationship { get; }

            public string outV { get; }

            public string inV { get; }
        }
    }
}