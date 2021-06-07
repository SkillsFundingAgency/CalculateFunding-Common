using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetIndex
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("changeNote")]
        public string ChangeNote { get; set; }

        [JsonProperty("changeType")]
        public string ChangeType { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("fundingPeriodNames")]
        public string[] FundingPeriodNames { get; set; }

        [JsonProperty("fundingperiodIds")]
        public string[] FundingPeriodIds { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("definitionName")]
        public string DefinitionName { get; set; }

        [JsonProperty("definitionId")]
        public string DefinitionId { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public DateTimeOffset LastUpdatedDate { get; set; }

        [JsonProperty("lastUpdatedByName")]
        public string LastUpdatedByName { get; set; }

        [JsonProperty("lastUpdatedById")]
        public string LastUpdatedById{ get; set; }

        [JsonProperty("specificationIds")]
        public string[] SpecificationIds { get; set; }

        [JsonProperty("specificationNames")]
        public string[] SpecificationNames { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingStreamName")]
        public string FundingStreamName { get; set; }
    }
}