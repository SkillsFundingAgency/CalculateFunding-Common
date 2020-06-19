using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    [Serializable]
    public class CalculationResult
    {
        [JsonProperty("id")] 
        public string Id { get; set; }

        [JsonProperty("value")] 
        public object Value { get; set; }
    }
}