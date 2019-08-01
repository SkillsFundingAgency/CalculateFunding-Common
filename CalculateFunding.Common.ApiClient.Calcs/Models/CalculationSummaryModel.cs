using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationSummaryModel : Reference
    {
        [JsonProperty("calculationType")]
        public CalculationType CalculationType { get; set; }

        [JsonProperty("status")]
        public PublishStatus Status { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}
