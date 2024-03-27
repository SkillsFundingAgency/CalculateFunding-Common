using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class FundingDataDefinition
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("identifierFieldType")]
        public object IdentifierFieldType { get; set; }
        [JsonProperty("dataType")]
        public string DataType { get; set; }
    }
}
