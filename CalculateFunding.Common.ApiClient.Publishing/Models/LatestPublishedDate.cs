using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class LatestPublishedDate
    {
        [JsonProperty("value")]
        public DateTime? Value { get; set; }
    }
}