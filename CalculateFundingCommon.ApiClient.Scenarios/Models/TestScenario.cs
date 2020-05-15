using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Scenarios.Models
{
    public class TestScenario : Reference
    {
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("current")]
        public TestScenarioVersion Current { get; set; }
    }
}