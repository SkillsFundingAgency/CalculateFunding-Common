using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Publishing.Models.Reprofiling
{
    public class ReprofilingSummaryResult
    {
        [JsonProperty("totalProviders")]
        public int TotalProviders;

        [JsonProperty("totalEligibleProviders")]
        public int TotalEligibleProviders;

        [JsonProperty("ProviderSummaryResult")]
        public List<ProviderSummaryResult> ProviderSummaryResult;


        [JsonProperty("Url")]
        public string Url { get; set; }
    }
}
