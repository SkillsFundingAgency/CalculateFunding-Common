using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Providers.Models
{
    public class ProviderSnapShotByFundingPeriod
    {
        [JsonProperty("fundingPeriodName")]
        public string FundingPeriodName { get; set; }

        [JsonProperty("providerSnapshotId")]
        public int? ProviderSnapshotId { get; set; }

        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }
    }
}
