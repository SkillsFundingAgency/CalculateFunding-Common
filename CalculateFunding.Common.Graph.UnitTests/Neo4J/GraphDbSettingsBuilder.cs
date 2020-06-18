using CalculateFunding.Common.Graph.Neo4J;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.Graph.UnitTests.Neo4J
{
    public class GraphDbSettingsBuilder
    {
        public GraphDbSettings Build()
        {
            return new GraphDbSettings
            {
                Url = "bolt://",
                Username = new RandomString(),
                Password = new RandomString()
            };
        }
    }
}