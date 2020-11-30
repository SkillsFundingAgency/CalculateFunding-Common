using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    [JsonConverter(typeof(StringEnumConverter))]

    public enum UpdateCoreProviderVersion
    {
        Manual = 0,
        ToLatest = 1,
        Paused = 2
    }
}