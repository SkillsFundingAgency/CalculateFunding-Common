using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Models
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