using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetUpgradeRequestModel
    {
        [JsonProperty("currentDataschemaId")]
        public string CurrentDataschemaId { get; set; }
        [JsonProperty("newDataschemaId")]
        public string NewDataschemaId { get; set; }
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }
        [JsonProperty("relationshipId")]
        public string RelationshipId { get; set; }
        [JsonProperty("includeAffectedCalcs")]
        public bool IncludeAffectedCalcs { get; set; }
        [JsonProperty("removedFields")]
        public List<string> RemovedFields { get; set; }
    }
}
