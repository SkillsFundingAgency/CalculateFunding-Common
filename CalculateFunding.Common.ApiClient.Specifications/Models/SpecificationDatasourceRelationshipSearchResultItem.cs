using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class SpecificationDatasourceRelationshipSearchResultItem
    {
        [JsonProperty("specificationId")]
        public string Id { get; set; }

        [JsonProperty("specificationName")]
        public string Name { get; set; }

        [JsonProperty("definitionRelationshipCount")]
        public int RelationshipCount { get; set; }
    }
}
