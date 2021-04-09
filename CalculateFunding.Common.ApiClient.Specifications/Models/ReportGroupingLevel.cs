using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReportGroupingLevel
    {
        Undefined,
        Current,
        All,
        Released
    }
}