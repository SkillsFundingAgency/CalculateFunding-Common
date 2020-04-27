using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    public class DataField
    {

        [JsonProperty("datafieldrelationshipname")]
        public string DataFieldRelationshipName { get; set; }
        [JsonProperty("specificationid")]
        public string SpecificationId { get; set; }

        [JsonProperty("calculationid")]
        public string CalculationId { get; set; }

        [JsonProperty("propertyname")]
        public string PropertyName { get; set; }

        [JsonProperty("datasetrelationshipid")]
        public string DatasetRelationshipId { get; set; }
        [JsonProperty("schemaid")]
        public string SchemaId { get; set; }
        [JsonProperty("schemaFieldid")]
        public string SchemaFieldId { get; set; }
        [JsonProperty("datafieldname")]
        public string DataFieldName { get; set; }
        [JsonProperty("datafieldid")]
        public string DataFieldId { get; set; }
        [JsonProperty("datafieldisaggregable")]
        public bool DataFieldIsAggregable { get; set; }

    }
}
