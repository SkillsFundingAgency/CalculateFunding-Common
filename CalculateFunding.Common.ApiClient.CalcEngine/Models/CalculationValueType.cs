using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.CalcEngine.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationValueType
    {
        Number,
        Percentage,
        Currency,
        Boolean,
        String
    }
}
