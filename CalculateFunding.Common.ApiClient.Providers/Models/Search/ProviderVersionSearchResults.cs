using CalculateFunding.Common.Models.Search;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Providers.Models.Search
{
    public class ProviderVersionSearchResults
    {
        public ProviderVersionSearchResults()
        {
            Results = new List<ProviderVersionSearchResult>();
            Facets = new List<Facet>();
        }

        [JsonProperty("totalCount")]
        public int TotalCount { get; set; }

        [JsonProperty("results")]
        public IEnumerable<ProviderVersionSearchResult> Results { get; set; }

        [JsonProperty("facets")]
        public IEnumerable<Facet> Facets { get; set; }
    }
}
