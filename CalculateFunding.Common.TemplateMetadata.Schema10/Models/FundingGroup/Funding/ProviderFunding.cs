using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// A funding item.
    /// </summary>
    public class ProviderFunding
    {
        /// <summary>
        /// A unique id for this funding. In format TODO fill in.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Version number of the published data. If there are changes to the funding for this organisation in this period, this number would increase.
        /// </summary>
        [JsonProperty("fundingVersion")]
        public string FundingVersion { get; set; }

        /// <summary>
        /// The organisation for which the funding is for.
        /// </summary>
        [JsonProperty("organisation")]
        public Organisation Organisation { get; set; }

        /// <summary>
        /// Optional variation details. Null if not appplicable (i.e. its version 1 of funding).
        /// </summary>
        [JsonProperty("variations", NullValueHandling = NullValueHandling.Ignore)]
        public Variations Variations { get; set; }

        /// <summary>
        /// The funding stream the funding relates to.
        /// </summary>
        [JsonProperty("fundingStreamCode")]
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// The funding period the funding relates to.
        /// </summary>
        [JsonProperty("fundingPeriodCode")]
        public string FundingPeriodCode { get; set; }

        /// <summary>
        /// Funding value.
        /// </summary>
        [JsonProperty("fundingValue")]
        public FundingValue FundingValue { get; set; }
    }
}