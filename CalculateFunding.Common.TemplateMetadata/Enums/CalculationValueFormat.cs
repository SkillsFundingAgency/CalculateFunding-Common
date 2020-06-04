using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationValueFormat
    {
        /// <summary>
        /// A number (e.g. a pupil number). For example, for 5 the return value should be 5 and 2.7334 should return 2.7334. Values are represented as a decimal.
        /// </summary>
        Number,

        /// <summary>
        /// A percentage amount. For example, for 25% the return value should be 25.
        /// </summary>
        Percentage,

        /// <summary>
        /// A currency. For example, for £5.83 the return value should be 5.83. Values are represented as a decimal.
        /// </summary>
        Currency,

        /// <summary>
        /// A boolean value. Values are represented as Javascript true or false values.
        /// </summary>
        Boolean,

        /// <summary>
        /// A string value.
        /// </summary>
        String
    }
}