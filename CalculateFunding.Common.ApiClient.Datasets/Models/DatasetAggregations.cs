using System.Collections.Generic;
using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetAggregations : IIdentifiable
    {
        [JsonProperty("id")]
        public string Id => $"{SpecificationId}_{DatasetRelationshipId}";

        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("datasetRelationshipId")]
        public string DatasetRelationshipId { get; set; }

        [JsonProperty("fields")]
        public IEnumerable<AggregatedField> Fields { get; set; }
    }
}