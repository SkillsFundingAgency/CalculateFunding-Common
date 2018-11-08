namespace CalculateFunding.Common.ApiClient.Models
{
    using System.Collections.Generic;

    public class SearchRequestModel
    {
        public int? PageNumber { get; set; }

        public string SearchTerm { get; set; }

        public bool IncludeFacets { get; set; }

        public IDictionary<string, string[]> Filters { get; set; }
    }
}
