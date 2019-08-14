using Newtonsoft.Json;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using System.ComponentModel.DataAnnotations;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
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
            AggregationType = AggregationType.Sum;
        }

        /// <summary>
        /// The name of this reference data (e.g. 'Academic year 2018 to 2019 pupil number on roll').
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

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
        /// The format of the reference data value (e.g. Percentage).
        /// </summary>
        [EnumDataType(typeof(ReferenceDataValueFormat))]
        [JsonProperty("format")]
        public ReferenceDataValueFormat Format { get; set; }

        /// <summary>
        /// The reference data value.
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }

        /// <summary>
        /// How the reference data should aggregate.
        /// </summary>
        [JsonProperty("aggregationType")]
        [EnumDataType(typeof(AggregationType))]
        public AggregationType AggregationType { get; set; }
    }
}