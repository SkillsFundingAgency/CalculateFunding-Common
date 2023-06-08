using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class FDSFieldDefinition
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("identifierFieldType")]
        public string IdentifierFieldType { get; set; }

        [JsonProperty("matchExpression")]
        public string MatchExpression { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("required")]
        public bool Required { get; set; }

        [JsonProperty("min")]
        public string Min { get; set; }

        [JsonProperty("max")]
        public string Max { get; set; }

        [JsonProperty("mustMatch")]
        public string MustMatch { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdDt")]
        public DateTime CreatedDt { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("isAggregable")]
        public object IsAggregable { get; set; }
    }
}