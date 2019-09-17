using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FieldType
    {
        Boolean,
        Char,
        Byte,
        Integer,
        Float,
        Decimal,
        DateTime,
        String,
        NullableOfInteger,
        NullableOfDecimal
    }
}