using System.Collections.Generic;
using System.Linq;

namespace CalculateFundingCommon.ApiClient.Scenarios.Models
{
    public class ScenarioSearchResults
    {
        public ScenarioSearchResults()
        {
            Results = Enumerable.Empty<ScenarioSearchResult>();
        }

        public IEnumerable<ScenarioSearchResult> Results { get; set; }

        public int TotalCount { get; set; }
    }
}