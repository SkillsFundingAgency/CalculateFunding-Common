using System.ComponentModel.DataAnnotations;
using CalculateFunding.Generators.OrganisationGroup.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Generators.OrganisationGroup.Models
{
    /// <summary>
    /// A key/vaue pairing representing a organisation identifier.
    /// </summary>
    public class OrganisationIdentifier
    {
        /// <summary>
        /// The type of orgranisation identifier (e.g. UKPRN). 
        /// </summary>
        [EnumDataType(typeof(OrganisationGroupTypeIdentifier))]
        [JsonProperty("type")]
        public OrganisationGroupTypeIdentifier Type { get; set; }

        /// <summary>
        /// The value of this identifier type (e.g. if the type is UKPRN, then the value may be 12345678. 
        /// If the type is LECode, the value may be 'LA 203').
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
