using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetDefinitionIndex
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("providerIdentifier")]
        public string ProviderIdentifier { get; set; }

        [JsonProperty("modelHash")]
        public string ModelHash { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}