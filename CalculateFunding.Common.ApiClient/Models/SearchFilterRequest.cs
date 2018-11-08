namespace CalculateFunding.Common.ApiClient.Models
{
    using System.Collections.Generic;

    public class SearchFilterRequest : PagedQueryOptions
    {
        public string SearchTerm { get; set; }

        public bool IncludeFacets { get; set; }

        public IDictionary<string, string[]> Filters { get; set; }

        public IEnumerable<string> SearchFields { get; set; }

        public int FacetCount { get; set; }
    }
}
