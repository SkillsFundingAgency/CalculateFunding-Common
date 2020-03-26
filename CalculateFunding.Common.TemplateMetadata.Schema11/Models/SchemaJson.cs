using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Models
{
    public class SchemaJson
    {
        [JsonProperty("$schema")]
        public string Schema { get; set; }
        public string SchemaVersion { get; set; }
        public SchemaJsonFundingStreamTemplate FundingStreamTemplate { get; set; }
    }
}