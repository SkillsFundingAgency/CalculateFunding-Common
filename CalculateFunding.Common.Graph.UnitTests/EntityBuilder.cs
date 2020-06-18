using System.Collections.Generic;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.Graph.UnitTests
{
    public class EntityBuilder<T> : TestEntityBuilder
        where T : class
    {
        private T _node;
        private IEnumerable<Relationship> _relationships;
        
        public EntityBuilder<T> WithNode(T node)
        {
            _node = node;

            return this;
        }

        public EntityBuilder<T> WithRelationships(params Relationship[] relationships)
        {
            _relationships = relationships;

            return this;
        }
        
        public Entity<T> Build()
        {
            return new Entity<T>
            {
                Node = _node,
                Relationships = _relationships
            };
        }
    }
}