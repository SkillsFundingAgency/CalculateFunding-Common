using CalculateFunding.Common.Graph.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculateFunding.Common.Graph
{
    public class Entity<TNode>
        where TNode:class
    {
        public TNode Node { get; set; }

        public IEnumerable<IRelationship> Relationships { get; set; }
    }
}
