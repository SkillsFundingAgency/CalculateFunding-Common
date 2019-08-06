using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class DistributionPeriods
    {
        [JsonProperty("distributionPeriodCode")]
        public string DistributionPeriodCode { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }
    }
}
