using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class BatchProfilingRequestModel : ProfilingRequestModelBase
    {
        public IEnumerable<decimal> FundingValues { get; set; } 
    }
}