using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class DataField
    {
        [JsonProperty("datafieldid")]
        public string DataFieldId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }  
        
        [JsonProperty("fieldName")]
        public string FieldName { get; set; }
    }
}