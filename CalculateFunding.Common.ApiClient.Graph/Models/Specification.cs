using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    public class Specification
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("specificationid")]
        public string SpecificationId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
