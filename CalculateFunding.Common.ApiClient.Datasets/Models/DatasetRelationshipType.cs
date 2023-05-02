using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DatasetRelationshipType
    {
        Uploaded = 0,
        ReleasedData = 1,
        FDS = 2
    }
}