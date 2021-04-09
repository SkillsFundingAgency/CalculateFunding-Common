using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReportGrouping
    {
        Undefined,
        Live,
        Provider,
        Group,
        Profiling,
    }
}