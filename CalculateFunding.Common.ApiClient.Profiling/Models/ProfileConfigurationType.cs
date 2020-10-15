using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProfileConfigurationType
    {
        RuleBased,
        Custom,
    }
}