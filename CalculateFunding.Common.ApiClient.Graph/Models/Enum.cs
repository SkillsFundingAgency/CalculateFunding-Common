﻿using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class Enum
    {
        public const string IdField = "enumid";

        [JsonProperty(IdField)]
        public string EnumId => $"{SpecificationId}-{FundingStreamId}-{EnumName}-{EnumValue}";
        
        [JsonProperty("specificationid")]
        public string SpecificationId { get; set; }

        [JsonProperty("fundingstreamid")]
        public string FundingStreamId { get; set; }

        [JsonProperty("codereference")]
        public string CodeReference => $"{EnumName}.{EnumValue}";

        [JsonProperty("enumname")]
        public string EnumName { get; set; }

        [JsonProperty("enumvalue")]
        public string EnumValue { get; set; }

        [JsonProperty("enumvaluename")]
        public string EnumValueName { get; set; }
    }
}