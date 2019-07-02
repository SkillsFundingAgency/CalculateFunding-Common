using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class ProviderLookup
    {
        [JsonProperty("providerType")]
        public string ProviderType { get; set; }

        [JsonProperty("providerSubType")]
        public string ProviderSubType { get; set; }
    }
}
