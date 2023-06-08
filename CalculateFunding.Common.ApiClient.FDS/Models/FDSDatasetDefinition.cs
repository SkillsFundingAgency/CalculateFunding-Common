using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class FDSDatasetDefinition
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("fundingStreamCode")]
        public string FundingStreamCode { get; set; }

        [JsonProperty("fundingStreamName")]
        public string FundingStreamName { get; set; }

        [JsonProperty("fundingPeriodCode")]
        public string FundingPeriodCode { get; set; }

        [JsonProperty("fundingPeriodName")]
        public string FundingPeriodName { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdDt")]
        public DateTime CreatedDt { get; set; }

        [JsonProperty("comment")]
        public object Comment { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("tableDefinitions")]
        public List<FDSTableDefinitions> FDSTableDefinitions { get; set; }

    }
}