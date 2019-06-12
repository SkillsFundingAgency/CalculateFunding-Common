using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Providers.ViewModels
{
    public class MasterProviderVersionViewModel
    {
        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }
    }
}
