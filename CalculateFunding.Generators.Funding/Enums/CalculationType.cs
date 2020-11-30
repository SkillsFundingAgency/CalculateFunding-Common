using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Generators.Funding.Enums
{
    /// <summary>
    /// Valid list of calculation types.
    /// </summary>

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationType
    {
        /// <summary>
        /// A monetry value, not multipled by anything.
        /// </summary>
        [EnumMember(Value = "Cash")]
        Cash,

        /// <summary>
        /// Cash per paid X.
        /// </summary>
        [EnumMember(Value = "Rate")]
        Rate,

        /// <summary>
        /// Number of pupils.
        /// </summary>
        [EnumMember(Value = "Pupil Numbers")]
        PupilNumber,

        /// <summary>
        /// Number
        /// </summary>
        [EnumMember(Value = "Number")]
        Number,

        /// <summary>
        /// A number between 0 and 1.
        /// </summary>
        [EnumMember(Value = "Weighting")]
        Weighting,

        /// <summary>
        /// Boolean
        /// </summary>
        [EnumMember(Value = "Boolean")]
        Boolean,

        /// <summary>
        /// Enum
        /// </summary>
        [EnumMember(Value = "Enum")]
        Enum,
    }
}