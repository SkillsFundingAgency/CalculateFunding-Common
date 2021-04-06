using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class DatasetDefinition : SpecificationNode
    {
        [JsonProperty("datasetdefinitionid")]
        public string DatasetDefinitionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }   
        
        [JsonProperty("description")]
        public string Description { get; set; }    
    }
}