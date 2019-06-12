using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Providers.Models
{
    public class MasterProviderVersion : ProviderVersionMetadata, IIdentifiable
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
