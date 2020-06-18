using CalculateFunding.Common.Graph.Interfaces;

namespace CalculateFunding.Common.Graph
{
    public class Field : IField
    {
        public string Name { get; set; }
        
        public string Value { get; set; }
    }
}
