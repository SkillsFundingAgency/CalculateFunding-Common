using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class ProviderWithResultsForSpecifications
    {
        [JsonProperty("id")] 
        public string Id => Provider.Id;
            
        [JsonProperty("provider")]
        public ProviderInformation Provider { get; set; }
        
        [JsonProperty("specifications")]
        public IEnumerable<SpecificationInformation> Specifications { get; set; }
    }
}