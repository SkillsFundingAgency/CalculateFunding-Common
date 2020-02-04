using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Models
{
    public class SmokeResponse
    {
        [JsonProperty("listener")]
        public string Listener { get; set; }

        [JsonProperty("invocationId")]
        public string InvocationId { get; set; }

        [JsonProperty("service")]
        public string Service { get; set; }
    }
}
