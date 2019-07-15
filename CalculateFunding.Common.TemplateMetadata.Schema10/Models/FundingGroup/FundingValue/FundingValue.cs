using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// The total amount paid, and the periods/envelopes that it was composed of.
    /// </summary>
    public class FundingValue
    {
        /// <summary>
        /// The funding value amount in pence. Rolled up from all child Funding Lines where Type = Payment
        /// </summary>
        [JsonProperty("totalValue")]
        public long TotalValue { get; set; }

        /// <summary>
        /// An array showing the amounts by the periods (envelopes) they are paid in (e.g. for PE + Sport there are 2 periods per year, with a 7/5 split).
        /// </summary>
        [JsonProperty("fundingValueByDistributionPeriod")]
        public IEnumerable<FundingValueByDistributionPeriod> FundingValueByDistributionPeriod { get; set; }
    }
}