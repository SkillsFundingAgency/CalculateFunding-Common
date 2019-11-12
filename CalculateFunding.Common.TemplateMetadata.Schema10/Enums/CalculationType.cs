using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Enums
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
        Cash,

        /// <summary>
        /// Cash per paid X.
        /// </summary>
        Rate,

        /// <summary>
        /// Number of pupils.
        /// </summary>
        PupilNumber,

        /// <summary>
        /// A number between 0 and 1.
        /// </summary>
        Weighting,

        /// <summary>
        /// Work out eligibility (0 or 1).
        /// </summary>
        Scope,

        /// <summary>
        /// Informational information only.
        /// </summary>
        Information,

        /// <summary>
        /// Drilldown
        /// </summary>
        Drilldown,

        /// <summary>
        /// Per Pupil Funding
        /// </summary>
        PerPupilFunding,

        /// <summary>
        /// Lump Sum
        /// </summary>
        LumpSum,

        /// <summary>
        /// Provider Led Funding
        /// </summary>
        ProviderLedFunding,

        /// <summary>
        /// Number
        /// </summary>
        Number,
    }
}