using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.Models.Search;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class CalculationProviderResultSearchResults
    {
        public CalculationProviderResultSearchResults()
        {
            Results = Enumerable.Empty<CalculationProviderResultSearchResult>();
            Facets = Enumerable.Empty<Facet>();
        }

        public int TotalCount { get; set; }

        public int TotalErrorCount { get; set; }

        public IEnumerable<CalculationProviderResultSearchResult> Results { get; set; }

        public IEnumerable<Facet> Facets { get; set; }
    }
}