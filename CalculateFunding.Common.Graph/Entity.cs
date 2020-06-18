using System.Collections.Generic;
using CalculateFunding.Common.Graph.Interfaces;

namespace CalculateFunding.Common.Graph
{
    public class Entity<TNode>
        where TNode:class
    {
        public TNode Node { get; set; }

        public IEnumerable<IRelationship> Relationships { get; set; }

        public void AddRelationship(IRelationship relationship)
        {
            ((List<IRelationship>)(Relationships ??= new List<IRelationship>())).Add(relationship);
        }
    }
}
