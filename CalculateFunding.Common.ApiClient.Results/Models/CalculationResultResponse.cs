using Newtonsoft.Json;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class CalculationResultResponse
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

        [JsonProperty("calculationValueType")]
        public CalculationValueType CalculationValueType { get; set; }
    }
}
