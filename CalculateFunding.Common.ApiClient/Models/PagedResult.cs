namespace CalculateFunding.Common.ApiClient.Models
{
    using System.Collections.Generic;

    public class PagedResult<T>
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public int TotalErrorItems { get; set; }

        public IEnumerable<T> Items { get; set; }

        public IEnumerable<SearchFacet> Facets { get; set; }
    }
}
