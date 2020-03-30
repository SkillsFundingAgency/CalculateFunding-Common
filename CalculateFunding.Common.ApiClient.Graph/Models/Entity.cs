using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    public class Entity<TNode>
        where TNode:class
    {
        public TNode Node { get; set; }
        public IEnumerable<Relationship> Relationships { get; set; }
    }
}
