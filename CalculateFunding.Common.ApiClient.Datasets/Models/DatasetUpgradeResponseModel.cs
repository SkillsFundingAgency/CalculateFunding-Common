using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetUpgradeResponseModel
    {
        [JsonProperty("removedFields")]
        public IEnumerable<string> RemovedFields { get; set; }
        [JsonProperty("addedFields")]
        public IEnumerable<string> AddedFields { get; set; }
        [JsonProperty("obsoleteCalculations")]
        public ConcurrentDictionary<string, List<ObsoleteCalculationResponseModel>> ObsoleteCalculations { get; set; }
    }
}
