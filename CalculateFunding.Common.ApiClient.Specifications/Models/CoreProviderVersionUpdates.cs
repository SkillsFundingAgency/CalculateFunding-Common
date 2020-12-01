using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CoreProviderVersionUpdates
    {
        Manual = 0,
        UseLatest = 1
    }
}
