using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    public class Entity<TNode, TRelationship>
    {
        public TNode Node { get; set; }
        public TRelationship Relationship { get; set; }
    }
}
