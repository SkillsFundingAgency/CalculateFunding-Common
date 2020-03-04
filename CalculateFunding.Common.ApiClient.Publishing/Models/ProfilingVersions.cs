using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    [Serializable]
    public class ProfilingVersion
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("profiletotals")]
        public IEnumerable<ProfileTotal> ProfileTotals { get; set; }
    }
}
