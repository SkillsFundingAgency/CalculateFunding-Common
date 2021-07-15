using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class DatasetRelationship
    {
        [JsonProperty("datasetRelationshipId")]
        public string DatasetRelationshipId { get; set; }

        [JsonProperty("datasetRelationshipName")]
        public string DatasetRelationshipName { get; set; }

        [JsonProperty("datasetRelationshipType")]
        public DatasetRelationshipType DatasetRelationshipType { get; set; }

        [JsonProperty("schemaName")]
        public string SchemaName { get; set; }

        [JsonProperty("schemaId")]
        public string SchemaId { get; set; }
    }
}
