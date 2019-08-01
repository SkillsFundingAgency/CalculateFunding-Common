using CalculateFunding.Common.ApiClient.Calcs.Models.Schema;
using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class DatasetRelationshipSummary : Reference
    {
        [JsonProperty("relationship")]
        public Reference Relationship { get; set; }

        [JsonProperty("datasetDefinition")]
        public DatasetDefinition DatasetDefinition { get; set; }

        [JsonProperty("datasetDefinitionId")]
        public string DatasetDefinitionId { get; set; }

        [JsonProperty("dataGranularity")]
        public DataGranularity DataGranularity { get; set; }

        [JsonProperty("definesScope")]
        public bool DefinesScope { get; set; }

        [JsonProperty("datasetId")]
        public string DatasetId { get; set; }
    }
}