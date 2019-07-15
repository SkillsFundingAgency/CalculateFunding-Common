using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Enums
{
    /// <summary>
    /// The funding status (wether its released or otherwise).
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FundingStatus
    {
        /// <summary>
        /// Approved.
        /// </summary>
        Approved,

        /// <summary>
        /// Updated.
        /// </summary>
        Updated,

        /// <summary>
        /// Released - previously published.
        /// </summary>
        Released,
    }
}