using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AggregationType
    {
        None,
        Average,
        Sum,
        GroupRate,
        PercentageChangeBetweenAandB
    }
}