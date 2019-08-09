using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ProviderResult
    {
        [JsonProperty("providerId")] 
        public string ProviderId { get; set; }

        [JsonProperty("results")] 
        public IEnumerable<CalculationResult> Results { get; set; }
    }
}