﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TargetLanguage
    {
       // CSharp,
        VisualBasic
    }
}