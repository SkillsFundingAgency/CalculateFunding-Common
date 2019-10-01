using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationUpdateModel
    {
        [JsonProperty("allocationLineId")]
        public string AllocationLineId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("calculationType")]
        public CalculationSpecificationType CalculationType { get; set; }
    }
}
