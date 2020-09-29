using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProviderFundingStructure
    {
        [JsonProperty("items")]
        public IEnumerable<PublishedProviderFundingStructureItem> Items { get; set; }

        [JsonIgnore]
        public int PublishedProviderVersion { get; set; }
    }
}
