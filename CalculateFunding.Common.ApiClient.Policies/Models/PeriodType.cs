using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class PeriodType : Reference
    {
        [JsonProperty("startDay")]
        public int StartDay { get; set; }

        [JsonProperty("startMonth")]
        public int StartMonth { get; set; }

        [JsonProperty("endDay")]
        public int EndDay { get; set; }

        [JsonProperty("endMonth")]
        public int EndMonth { get; set; }
    }
}
