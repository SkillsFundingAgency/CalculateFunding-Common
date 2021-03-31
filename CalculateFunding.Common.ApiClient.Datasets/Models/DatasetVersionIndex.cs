using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetVersionIndex
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("datasetId")]
        public string DatasetId { get; set; }

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

        [JsonProperty("definitionName")]
        public string DefinitionName { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public DateTimeOffset LastUpdatedDate { get; set; }

        [JsonProperty("lastUpdatedByName")]
        public string LastUpdatedByName { get; set; }

        [JsonProperty("blobName")]
        public string BlobName { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingStreamName")]
        public string FundingStreamName { get; set; }
    }
}