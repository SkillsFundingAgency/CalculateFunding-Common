using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.Models.Search
{
    public class SearchModel
    {
        public SearchModel()
        {
            OrderBy = new List<string>();
            Filters = new Dictionary<string, string[]>();
            SearchFields = new List<string>();
            OverrideFacetFields = new List<string>();
        }

        public int PageNumber { get; set; }

        public int Top { get; set; }

        public string SearchTerm { get; set; } = "";

        public bool? ErrorToggle { get; set; }

        public IEnumerable<string> OrderBy { get; set; }

        public IDictionary<string, string[]> Filters { get; set; }

        public bool IncludeFacets { get; set; }

        public int FacetCount { get; set; } = 10;

        public bool CountOnly { get; set; }

        public SearchMode SearchMode { get; set; }

        public IEnumerable<string> SearchFields { get; set; }

        public IEnumerable<string> OverrideFacetFields { get; set; }
    }
}
