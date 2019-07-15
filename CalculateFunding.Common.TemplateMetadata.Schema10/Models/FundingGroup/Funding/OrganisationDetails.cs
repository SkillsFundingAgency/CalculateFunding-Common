using Newtonsoft.Json;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// (Optional) details about an organisation. Passed through from the provider API.
    /// </summary>
    public class OrganisationDetails
    {
        /// <summary>
        /// Date Opened.
        /// </summary>
        [JsonProperty("dateOpened")]
        public DateTimeOffset? DateOpened { get; set; }

        /// <summary>
        /// Date Closed.
        /// </summary>
        [JsonProperty("dateClosed")]
        public DateTimeOffset? DateClosed { get; set; }

        /// <summary>
        /// Status of the organisation (TODO find examples).
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// TODO: Find out if this is required in the logical model
        /// </summary>
        [JsonProperty("phaseOfEducation")]
        public string PhaseOfEducation { get; set; }

        /// <summary>
        /// Optional open reason from the list of GIAS Open Reasons
        /// </summary>
        [EnumDataType(typeof(ProviderOpenReason))]
        [JsonProperty("openReason")]
        public ProviderOpenReason? OpenReason { get; set; }

        /// <summary>
        /// Optional close reason from list of GIAS Close Reasons
        /// </summary>
        [EnumDataType(typeof(ProviderCloseReason))]
        [JsonProperty("closeReason")]
        public ProviderCloseReason? CloseReason { get; set; }

        /// <summary>
        /// TODO: Find out if this is required in the logical model
        /// </summary>
        [EnumDataType(typeof(TrustStatus))]
        [JsonProperty("trustStatus")]
        public TrustStatus TrustStatus { get; set; }

        /// <summary>
        /// TODO: Find out if this is required in the logical model
        /// </summary>
        [JsonProperty("trustName")]
        public string TrustName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("address")]
        public OrganisationAddress Address { get; set; }
    }
}