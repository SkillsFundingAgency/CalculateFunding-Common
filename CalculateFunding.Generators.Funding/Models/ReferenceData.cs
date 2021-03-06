﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CalculateFunding.Generators.Funding.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Generators.Funding.Models
{
    /// <summary>
    /// Data that is required by calculations.
    /// </summary>
    public class ReferenceData
    {
        /// <summary>
        ///  Create an instance of reference data, setting properties to defaults. 
        /// </summary>
        public ReferenceData()
        {
        }

        /// <summary>
        /// The template reference id (i.e. a way to get to this property in the template).
        /// This value can be the same for multiple references within the hierarchy. 
        /// This indicates they will return the same value from the output.
        /// It allows input template to link references together, so a single reference implemenation will be created instead of multiple depending on the hierarchy.
        /// 
        /// When templates are versioned, template IDs should be kept the same if they refer to the same thing, otherwise a new, unused ID should be used.
        /// </summary>
        [JsonProperty("templateReferenceId")]
        public uint TemplateReferenceId { get; set; }

        /// <summary>
        /// The reference data value.
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
