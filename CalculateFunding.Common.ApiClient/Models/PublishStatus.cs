
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PublishStatus
    {
        Draft,
        Approved,
        Updated,
        Archived
    }
}
