using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class SpecificationConverterMergeRequest
    {
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }
        
        [JsonProperty("author")]
        public Reference Author { get; set; }
    }
}