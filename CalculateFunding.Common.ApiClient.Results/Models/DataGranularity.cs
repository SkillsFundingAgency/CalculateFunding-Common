using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataGranularity
    {
        SingleRowPerProvider,
        MultipleRowsPerProvider
    }
}