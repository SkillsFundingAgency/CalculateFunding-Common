using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Providers.Models
{
    public class ProviderVersionMetadata
    {
        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }

        [JsonProperty("versionType")]
        public ProviderVersionType VersionType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("targetDate")]
        public DateTimeOffset TargetDate { get; set; }

        [JsonProperty("fundingStream")]
        public string FundingStream { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }

        [JsonProperty("validationResult")]
        public string ValidationResult { get; set; }

        [JsonIgnore]
        public bool IsValid => string.IsNullOrWhiteSpace(ValidationResult);
    }
}
