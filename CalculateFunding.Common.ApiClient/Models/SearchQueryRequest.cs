namespace CalculateFunding.Common.ApiClient.Models
{
    using System.Collections.Generic;
    using CalculateFunding.Common.Utility;

    public class SearchQueryRequest
    {
        public int PageNumber { get; set; }

        public int Top { get; set; }

        public string SearchTerm { get; set; }

        public bool? ErrorToggle { get; set; }

        public bool IncludeFacets { get; set; }

        public int FacetCount { get; set; }

        public IDictionary<string, string[]> Filters { get; set; }

        public SearchMode SearchMode { get; set; }

        public static SearchQueryRequest FromSearchFilterRequest(SearchFilterRequest filterOptions)
        {
            Guard.ArgumentNotNull(filterOptions, nameof(filterOptions));

            SearchQueryRequest result = new SearchQueryRequest()
            {
                PageNumber = filterOptions.Page,
                Top = filterOptions.PageSize,
                SearchTerm = filterOptions.SearchTerm,
                IncludeFacets = filterOptions.IncludeFacets,
                Filters = filterOptions.Filters,
                FacetCount = filterOptions.FacetCount,
                SearchMode = filterOptions.SearchMode,
                ErrorToggle = filterOptions.ErrorToggle
            };

            return result;
        }
    }
}
