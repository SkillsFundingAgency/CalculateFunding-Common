using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Enums
{
    /// <summary>
    /// A period that a funding line covers.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ProfilePeriodType
    {
        /// <summary>
        /// A calendar month.
        /// </summary>
        CalendarMonth,
    }
}
