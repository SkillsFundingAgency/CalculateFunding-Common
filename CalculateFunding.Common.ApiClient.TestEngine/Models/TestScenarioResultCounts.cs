using System;

namespace CalculateFunding.Common.ApiClient.TestEngine.Models
{
    public class TestScenarioResultCounts
    {
        public string TestScenarioId { get; set; }

        public string TestScenarioName { get; set; }

        public int Passed { get; set; }

        public int Failed { get; set; }

        public int Ignored { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}
