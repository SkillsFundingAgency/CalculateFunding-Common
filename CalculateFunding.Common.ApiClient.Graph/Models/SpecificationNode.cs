using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class SpecificationNode
    {
        [JsonProperty("specificationid")]
        public string SpecificationId { get; set; }
    }
}
