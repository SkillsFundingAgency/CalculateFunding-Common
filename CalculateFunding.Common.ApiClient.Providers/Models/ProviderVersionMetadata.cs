using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Providers.Models
{
    public class ProviderVersionMetadata
    {
        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }

        [JsonProperty("versionType")]
        public string ProviderVersionTypeString { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("created")]
        public DateTimeOffset Created { get; set; }
    }
}
