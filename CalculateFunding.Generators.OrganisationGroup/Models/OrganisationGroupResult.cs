using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Generators.OrganisationGroup.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Generators.OrganisationGroup.Models
{
    public class OrganisationGroupResult
    {
        /// <summary>
        /// Providers in this organisation group
        /// </summary>
        [JsonProperty("providers")]
        public IEnumerable<Provider> Providers { get; set; }

        /// <summary>
        /// The organisation group type. eg UKPRN or LACode
        /// </summary>
        [EnumDataType(typeof(OrganisationGroupTypeCode))]
        [JsonProperty("groupTypeCode")]
        public OrganisationGroupTypeCode GroupTypeCode { get; set; }

        /// <summary>
        /// The organisation group type. eg UKPRN or LACode
        /// </summary>
        [EnumDataType(typeof(OrganisationGroupTypeIdentifier))]
        [JsonProperty("groupTypeIdentifier")]
        public OrganisationGroupTypeIdentifier GroupTypeIdentifier { get; set; }

        /// <summary>
        /// Value of the organisation type key, eg the actual UKPRN or LACode. 100023 or 202
        /// </summary>
        [JsonProperty("identifierValue")]
        public string IdentifierValue { get; set; }

        /// <summary>
        /// The organisation group type. eg UKPRN or LACode
        /// </summary>
        [EnumDataType(typeof(OrganisationGroupTypeClassification))]
        [JsonProperty("groupTypeCategory")]
        public OrganisationGroupTypeClassification GroupTypeClassification { get; set; }

        /// <summary>
        /// The name of the grouping organisation (e.g. in the case of the type being LA, this could be 'Camden', "Bermondsey and Old Southwark").
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
    }
}
