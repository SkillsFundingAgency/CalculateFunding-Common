﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupingReason
    {
        Payment,
        Information,
        Contracting,
        Indicative
    }
}
