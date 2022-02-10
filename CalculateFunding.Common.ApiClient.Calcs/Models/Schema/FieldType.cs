﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Calcs.Models.Schema
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
        NullableOfDecimal,
        NullableOfBoolean,
    }
}