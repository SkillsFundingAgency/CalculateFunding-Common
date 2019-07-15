using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Enums
{
    /// <summary>
    /// The periods of time a period can relate to.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PeriodType
    {
        /// <summary>
        /// An academic year (early September till end of July).
        /// </summary>
        AcademicYear,

        /// <summary>
        /// A financial year (1 April to 31 March).
        /// </summary>
        FinancialYear,
    }
}