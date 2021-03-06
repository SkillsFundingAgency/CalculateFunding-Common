﻿using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class Enum : SpecificationNode
    {
        public const string IdField = "enumid";

        [JsonProperty(IdField)]
        public string EnumId => $"{SpecificationId}-{FundingStreamId}-{EnumName}-{EnumValue}";

        [JsonProperty("fundingstreamid")]
        public string FundingStreamId { get; set; }

        [JsonProperty("codereference")]
        public string CodeReference => $"{EnumName}.{EnumValue}";

        [JsonProperty("enumname")]
        public string EnumName { get; set; }

        [JsonProperty("enumvalue")]
        public string EnumValue { get; set; }

        [JsonIgnore]
        public string EnumValueName { get; set; }
    }
}
