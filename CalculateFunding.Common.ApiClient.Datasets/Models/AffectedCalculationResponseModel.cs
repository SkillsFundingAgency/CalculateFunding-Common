using CalculateFunding.Common.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class AffectedCalculationResponseModel : Reference
    {
        [JsonProperty("calculationType")]
        public string CalculationType { get; set; }

        [JsonProperty("valueType")]
        public string ValueType { get; set; }

        [JsonProperty("removedDataFields")]
        public IEnumerable<string> RemovedDataFields { get; set; }

    }
}
