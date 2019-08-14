using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// A funding item.
    /// </summary>
    public class ProviderFunding
    {
        /// <summary>
        /// A unique id for this funding.
        /// </summary>
        [JsonProperty("id")]
        public string Id => $"{FundingStreamCode}-{FundingPeriodId}-{Provider.Identifier}-{ConvertVersionForId(FundingVersion)}";

        /// <summary>
        /// Version number of the published data. If there are changes to the funding for this organisation in this period, this number would increase.
        /// </summary>
        [JsonProperty("fundingVersion")]
        public string FundingVersion { get; set; }

        /// <summary>
        /// The organisation for which the funding is for.
        /// </summary>
        [JsonProperty("provider")]
        public Provider Provider { get; set; }

        /// <summary>
        /// The funding stream the funding relates to.
        /// </summary>
        [JsonProperty("fundingStreamCode")]
        public string FundingStreamCode { get; set; }

        /// <summary>
        /// The funding period the funding relates to. eg AY-1819
        /// </summary>
        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        /// <summary>
        /// Funding value.
        /// </summary>
        [JsonProperty("fundingValue")]
        public FundingValue FundingValue { get; set; }

        /// <summary>
        /// Optional reasons for the provider variation. These reasons are in addition to open and close reason of the organisation.
        /// This field can contain zero or more items.
        /// </summary>
        [EnumDataType(typeof(VariationReason))]
        [JsonProperty("variationReasons")]
        public IEnumerable<VariationReason> VariationReasons { get; set; }

        /// <summary>
        /// Collection of successor providers
        /// </summary>
        [JsonProperty("successors")]
        public IEnumerable<ProviderInformationModel> Successors { get; set; }

        /// <summary>
        /// Collection of predecessor providers
        /// </summary>
        [JsonProperty("predecessors")]
        public IEnumerable<ProviderInformationModel> Predecessors { get; set; }

        private string ConvertVersionForId(string version)
        {
            return version.Replace(".", "_");
        }
    }
}