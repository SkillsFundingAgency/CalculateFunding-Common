using System.Collections.Generic;
using CalculateFunding.Common.ApiClient.Models;
using Newtonsoft.Json;

namespace CalculateFundingCommon.ApiClient.Scenarios.Models
{
    public class TestScenarioVersion : VersionedItem
    {
        //AB: These 2 properties are not required yet, will be updated during the story
        [JsonProperty("id")]
        public override string Id => $"{TestScenarioId}_version_{Version}";

        [JsonProperty("entityId")]
        public override string EntityId => $"{TestScenarioId}";

        [JsonProperty("testScenarioId")]
        public string TestScenarioId { get; set; }

        [JsonProperty("gherkin")]
        public string Gherkin { get; set; }

        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("fundingStreamIds")]
        public IEnumerable<string> FundingStreamIds { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public override VersionedItem Clone()
        {
            return JsonConvert.DeserializeObject<TestScenarioVersion>(JsonConvert.SerializeObject(this));
        }
    }
}