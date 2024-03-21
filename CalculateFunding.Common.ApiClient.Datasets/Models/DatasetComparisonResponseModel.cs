using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetComparisonResponseModel
    {
        [JsonProperty("removedFields")]
        public IEnumerable<DatasetComparisonField> RemovedFields { get; set; }
        [JsonProperty("addedFields")]
        public IEnumerable<DatasetComparisonField> AddedFields { get; set; }
        [JsonProperty("affectedCalculations")]
        public IEnumerable<AffectedCalculationResponseModel> AffectedCalculations { get; set; }
    }
}
