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

        [JsonProperty("exceptionStackTrace")]
        public string ExceptionStackTrace { get; set; }

        /// <summary>
        /// Elapsed time, used for debugging locally and shouldn't be stored in cosmos
        /// </summary>
        [JsonIgnore]
        [JsonProperty("elapsedTime")]
        public long ElapsedTime { get; set; }

        [JsonProperty("calculationType")]
        public CalculationType CalculationType { get; set; }

        [JsonProperty("calculationDataType")]
        public CalculationDataType CalculationDataType { get; set; }
    }
}