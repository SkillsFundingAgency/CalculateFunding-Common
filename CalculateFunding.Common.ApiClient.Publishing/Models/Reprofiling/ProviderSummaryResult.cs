using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Publishing.Models.Reprofiling
{
    public class ProviderSummaryResult
    {
        [JsonProperty("UKPRN")]
        public string UKPRN;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("openDate")]
        public DateTimeOffset? OpenDate;

        [JsonProperty("isEligible")]
        public bool IsEligible;

        [JsonProperty("errorMessage")]
        public string ErrorMessage;
    }
}
