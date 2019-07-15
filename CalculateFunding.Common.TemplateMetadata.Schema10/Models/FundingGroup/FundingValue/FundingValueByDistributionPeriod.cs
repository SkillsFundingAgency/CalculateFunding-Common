using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// Funding values grouped by the distribution period (envelope) they are paid in.
    /// </summary>
    public class FundingValueByDistributionPeriod
    {
        /// <summary>
        /// The overall value for the distribution period in pence. Rolled up from all child Funding Lines where Type = Payment
        /// </summary>
        [JsonProperty("value")]
        public long Value { get; set; }

        /// <summary>
        /// The funding period the funding relates to.
        /// </summary>
        [JsonProperty("distributionPeriodCode")]
        public string DistributionPeriodCode { get; set; }

        /// <summary>
        /// The lines that make up this funding. 
        /// </summary>
        [JsonProperty("fundingLines")]
        public IEnumerable<FundingLine> FundingLines { get; set; }
    }
}