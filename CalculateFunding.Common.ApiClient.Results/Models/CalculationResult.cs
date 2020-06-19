using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class CalculationResult
    {
        [JsonProperty("calculation")]
        public Reference Calculation { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }

        [JsonProperty("exceptionType")]
        public string ExceptionType { get; set; }

        [JsonProperty("exceptionMessage")]
        public string ExceptionMessage { get; set; }

        [JsonProperty("calculationType")]
        public CalculationType CalculationType { get; set; }
    }
}