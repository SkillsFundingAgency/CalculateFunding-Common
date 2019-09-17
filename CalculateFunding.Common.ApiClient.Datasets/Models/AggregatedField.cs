using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class AggregatedField
    {
        [JsonProperty("fieldType")]
        public AggregatedType FieldType { get; set; }

        [JsonProperty("value")]
        public decimal? Value { get; set; }

        [JsonProperty("fieldDefinitionName")]
        public string FieldDefinitionName { get; set; }

        [JsonIgnore]
        public string FieldReference => $"{FieldDefinitionName}_{FieldType.ToString()}";
    }
}