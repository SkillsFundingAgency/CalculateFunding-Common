using CalculateFunding.Common.Graph.Interfaces;

namespace CalculateFunding.Common.Graph
{
    public class Relationship : IRelationship
    {
        public dynamic One { get; set; }
        public dynamic Two { get; set; }
        public string Type { get; set; }
    }
}
