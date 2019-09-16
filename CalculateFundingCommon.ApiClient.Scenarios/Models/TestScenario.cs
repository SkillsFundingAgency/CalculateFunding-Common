using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFundingCommon.ApiClient.Scenarios.Models
{
    public class TestScenario : Reference
    {
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("current")]
        public TestScenarioVersion Current { get; set; }
    }
}