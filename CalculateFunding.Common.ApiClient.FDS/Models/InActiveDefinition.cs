using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class InActiveDefinition
    {
        [JsonProperty("definitionId")]
        public int DefinitionId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
