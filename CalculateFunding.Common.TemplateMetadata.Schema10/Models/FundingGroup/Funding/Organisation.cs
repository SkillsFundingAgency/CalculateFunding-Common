using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// An organisation.
    /// </summary>
    public class Organisation
    {
        /// <summary>
        /// The name of the organisation.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

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

        /// <summary>
        /// ID on the provider lookup API.
        /// </summary>
        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }

        /// <summary>
        /// Provider type (e.g. School, Academy, Special School) - not enumerated as this isn't controlled by CFS, but passed through from the Provider info (GIAS)
        /// </summary>
        [JsonProperty("providerType")]
        public string ProviderType { get; set; }

        /// <summary>
        /// Provider sub type (TODO - list enumerations).
        /// </summary>
        [JsonProperty("providerSubType")]
        public string ProviderSubType { get; set; }

        /// <summary>
        /// Organisation Details. This property is optional
        /// </summary>
        [JsonProperty("organisationDetails")]
        public OrganisationDetails OrganisationDetails { get; set; }
    }
}