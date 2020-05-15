using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.ApiClient.Scenarios.Models
{
    public class ScenarioSearchResults
    {
        public ScenarioSearchResults()
        {
            Results = new List<ScenarioSearchResult>();
        }

        public IEnumerable<ScenarioSearchResult> Results { get; set; }

        public int TotalCount { get; set; }
    }
}