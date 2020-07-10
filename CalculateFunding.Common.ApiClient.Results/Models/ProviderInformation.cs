using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class ProviderInformation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}