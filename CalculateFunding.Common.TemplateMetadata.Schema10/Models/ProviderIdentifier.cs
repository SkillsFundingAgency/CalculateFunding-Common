﻿using System.ComponentModel.DataAnnotations;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// A key/vaue pairing representing a provider identifier.
    /// </summary>
    public class ProviderIdentifier
    {
        /// <summary>
        /// The type of provider identifier (e.g. UKPRN). 
        /// </summary>
        [EnumDataType(typeof(ProviderTypeIdentifier))]
        [JsonProperty("type")]
        public ProviderTypeIdentifier Type { get; set; }

        /// <summary>
        /// The value of this identifier type (e.g. if the type is UKPRN, then the value may be 12345678. 
        /// If the type is LECode, the value may be 'LA 203').
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}