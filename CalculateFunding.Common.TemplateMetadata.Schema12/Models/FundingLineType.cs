﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema12.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FundingLineType
    {
        Payment,
        Information
    }
}