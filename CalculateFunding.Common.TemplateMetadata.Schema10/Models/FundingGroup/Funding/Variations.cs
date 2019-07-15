using Newtonsoft.Json;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// If funding value has changed, either the amounts have been updated, or the provider has closed/changed, this gives details as to why.
    /// </summary>
    public class Variations
    {
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
    }
}