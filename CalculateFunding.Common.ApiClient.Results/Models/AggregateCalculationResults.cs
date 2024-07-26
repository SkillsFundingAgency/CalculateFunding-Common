using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class AggregateCalculationResults
    {
        [JsonProperty("calculationId")]
        public string CalculationId { get; set; }

        [JsonProperty("sumValue")]
        public decimal SumValue { get; set; }

        [JsonProperty("avgValue")]
        public decimal AvgValue { get; set; }

        [JsonProperty("maxValue")]
        public decimal MaxValue { get; set; }

        [JsonProperty("minValue")]
        public decimal MinValue { get; set; }

    }
}
