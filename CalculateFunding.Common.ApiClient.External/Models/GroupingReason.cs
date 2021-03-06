using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.External.Models
{
    /// <summary>
    /// The reason for the grouping. Is it paid based on this grouping, or just informational.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupingReason
    {
        /// <summary>
        /// Paid in this way.
        /// </summary>
        Payment,

        /// <summary>
        /// Informational only.
        /// </summary>
        Information,

        /// <summary>
        /// Indicative
        /// </summary>
        Indicative,
        
        /// <summary>
        /// Contracting (a type of payment reason)
        /// </summary>
        Contracting
    }
}