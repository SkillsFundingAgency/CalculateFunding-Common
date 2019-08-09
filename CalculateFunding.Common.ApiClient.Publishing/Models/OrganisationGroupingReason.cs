using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    /// <summary>
    /// The funding line type (actual payment or informational only).
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrganisationGroupingReason
    {
        /// <summary>
        /// An actual payment.
        /// </summary>
        Payment,

        /// <summary>
        /// Information
        /// </summary>
        Information,
    }
}
