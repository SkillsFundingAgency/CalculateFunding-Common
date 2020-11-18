using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.CalcEngine.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationDataType
    {
        Decimal,
        String,
        Boolean,
        Enum
    }
}