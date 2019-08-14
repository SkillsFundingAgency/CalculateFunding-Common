using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Enums
{
    /// <summary>
    /// The funding line type (actual payment or informational only).
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FundingLineType
    {
        /// <summary>
        /// An actual payment.
        /// </summary>
        Payment,

        /// <summary>
        /// ,
        /// </summary>
        Information,
    }
}
