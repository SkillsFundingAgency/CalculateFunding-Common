using Newtonsoft.Json;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// A grouping organistion (e.g. 'Camden', an LA).
    /// </summary>
    public class OrganisationGroup
    {
        /// <summary>
        /// The name of the grouping organisation (e.g. in the case of the type being LA, this could be 'Camden').
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The organisation group type.
        /// </summary>
        [EnumDataType(typeof(OrganisationType))]
        [JsonProperty("type")]
        public OrganisationType Type { get; set; }

        /// <summary>
        /// Text for Azure search to make this entity searchable. This is the name, but with punctuation etc removed to make it suitable for searching
        /// </summary>
        [JsonProperty("searchableName")]
        public string SearchableName { get; set; }

        /// <summary>
        /// Identifier numbers for this organisation.
        /// </summary>
        [JsonProperty("identifiers")]
        public IEnumerable<OrganisationIdentifier> Identifiers { get; set; }
    }
}