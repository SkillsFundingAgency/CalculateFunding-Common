using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Jobs.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OutcomeType
    {
        Succeeded,
        ValidationError,
        UserError,
        Inconclusive,
        Failed
    }
}
