using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class Dataset
    {
        [JsonProperty("datasetid")]
        public string DatasetId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }    
        
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}