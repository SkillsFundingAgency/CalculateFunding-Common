using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationType
    {
        Additional,
        Template
    }
}
