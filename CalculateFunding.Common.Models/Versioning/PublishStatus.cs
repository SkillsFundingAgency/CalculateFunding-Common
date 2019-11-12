using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.Models.Versioning
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
