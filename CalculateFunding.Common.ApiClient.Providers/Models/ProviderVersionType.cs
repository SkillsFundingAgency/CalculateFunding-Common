using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Providers.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProviderVersionType
    {
        Missing,
        Custom,
        SystemImported,
    }
}
