
using CalculateFunding.Common.Graph;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.Graph.UnitTests
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