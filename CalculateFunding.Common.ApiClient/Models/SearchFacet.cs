namespace CalculateFunding.Common.ApiClient.Models
{
    using System.Collections.Generic;

    public class SearchFacet
    {
        public string Name { get; set; }

        public IEnumerable<SearchFacetValue> FacetValues { get; set; }
    }
}
