using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ProfilePatternOpenDateConfiguration
    {
        [JsonProperty("openDateStart")]
        public DateTime OpenDateStart { get; set; }

        [JsonProperty("openDateEnd")]
        public DateTime OpenDateEnd { get; set; }

        [JsonProperty("providerType")]
        public string ProviderType { get; set; }
    }
}
