using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    public class DatasetField
    {

        [JsonProperty("datasetfieldrelatioshipname")]
        public string DatasetFieldRelatioshipName { get; set; }
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
        [JsonProperty("datasetfieldname")]
        public string DatasetFieldName { get; set; }
        [JsonProperty("datasetfieldid")]
        public string DatasetFieldId { get; set; }
        [JsonProperty("datasetfieldisaggregable")]
        public bool DatasetFieldIsAggregable { get; set; }

    }
}
