using System;

namespace CalculateFunding.Common.ApiClient.TestEngine.Models
{
    public class ScenarioResultCounts
    {
        public int Passed { get; set; }

        public int Failed { get; set; }

        public int Ignored { get; set; }

        public decimal TestCoverage
        {
            get
            {
                int totalRecords = Passed + Failed + Ignored;

                return (totalRecords == 0)
                    ? 0
                    : Math.Round((decimal)(Passed + Failed) / totalRecords * 100, 1);
            }
        }
    }
}