using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class Enum
    {
        public const string IdField = "enumid";

        [JsonProperty(IdField)]
        public string EnumId => $"{EnumName}.{EnumValue}";

        [JsonProperty("enumname")]
        public string EnumName { get; set; }

        [JsonProperty("enumvalue")]
        public string EnumValue { get; set; }

        [JsonProperty("enumvaluename")]
        public string EnumValueName { get; set; }
    }
}
