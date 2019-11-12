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
        /// A number between 0 and 1.
        /// </summary>
        [EnumMember(Value = "Weighting")]
        Weighting,

        /// <summary>
        /// Work out eligibility (0 or 1).
        /// </summary>
        [EnumMember(Value = "Scope")]
        Scope,

        /// <summary>
        /// Informational information only.
        /// </summary>
        [EnumMember(Value = "Information")]
        Information,

        /// <summary>
        /// Drilldown
        /// </summary>
        [EnumMember(Value = "Drilldown")]
        Drilldown,

        /// <summary>
        /// Per Puple Funding
        /// </summary>
        [EnumMember(Value = "Per Pupil Funding")]
        PerPupilFunding,

        /// <summary>
        /// Lump Sum
        /// </summary>
        [EnumMember(Value = "Lump Sum")]
        LumpSum,

        /// <summary>
        /// Provider Led Funding
        /// </summary>
        [EnumMember(Value = "Provider Led Funding")]
        ProviderLedFunding,

        /// <summary>
        /// Number
        /// </summary>
        [EnumMember(Value = "Number")]
        Number,
    }
}