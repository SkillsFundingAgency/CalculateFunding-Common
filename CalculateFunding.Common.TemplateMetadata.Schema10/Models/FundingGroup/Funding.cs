using System;
using System.ComponentModel.DataAnnotations;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// A funding group (a parent grouping organisation - such as an LA, MAT, Region etc...).
    /// </summary>
    public abstract class Funding
    {
        /// <summary>
        /// Unique identifier of this funding group / business event (in format 'TODO').
        /// </summary>
        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        /// <summary>
        /// Version number of the published data. If there are changes to the funding for this organisation in this period, this number would increase.
        /// </summary>
        [JsonProperty("fundingVersion", Order = 2)]
        public string FundingVersion { get; set; }

        /// <summary>
        /// The funding status (i.e. published).
        /// </summary>
        [EnumDataType(typeof(FundingStatus))]
        [JsonProperty("status", Order = 3)]
        public FundingStatus Status { get; set; }

        /// <summary>
        /// The funding stream the funding relates to.
        /// </summary>
        [JsonProperty("fundingStream", Order = 4)]
        public StreamWithTemplateVersion FundingStream { get; set; }

        /// <summary>
        /// The funding period the funding relates to.
        /// </summary>
        [JsonProperty("fundingPeriod", Order = 5)]
        public FundingPeriod FundingPeriod { get; set; }

        /// <summary>
        /// The grouped organisation or region (e.g. if we are grouping by LA, the organisation may be Camden).
        /// </summary>
        [JsonProperty("organisationGroup", Order = 6)]
        public OrganisationGroup OrganisationGroup { get; set; }

        /// <summary>
        /// Funding value breakdown
        /// </summary>
        [JsonProperty("fundingValue", Order = 7)]
        public FundingValue FundingValue { get; set; }

        /// <summary>
        /// Does the grouping reflect how the money is paid ('Payment') or is it just useful to show it this way? ('Informational'). 
        /// </summary>
        [EnumDataType(typeof(GroupingReason))]
        [JsonProperty("groupingReason", Order = 9)]
        public GroupingReason GroupingReason { get; set; }

        /// <summary>
        /// The date the funding was published by a business user.
        /// </summary>
        [JsonProperty("statusChangedDate", Order = 10)]
        public DateTimeOffset StatusChangedDate { get; set; }

        /// <summary>
        /// Date and time when the allocation can be published externally.
        /// </summary>
        [JsonProperty("externalPublicationDate", Order = 11)]
        public DateTimeOffset ExternalPublicationDate { get; set; }

        /// <summary>
        /// The date the payment will be made to the provider.
        /// </summary>
        [JsonProperty("paymentDate", Order = 12)]
        public DateTimeOffset? PaymentDate { get; set; }
    }
}