
using System;

namespace CalculateFundingCommon.ApiClient.Scenarios.Models
{
    public class ScenarioSearchResult
    {
        public string Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string SpecificationName { get; set; }
        
        public string FundingPeriodName { get; set; }
        
        public string Status { get; set; }
        
        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}