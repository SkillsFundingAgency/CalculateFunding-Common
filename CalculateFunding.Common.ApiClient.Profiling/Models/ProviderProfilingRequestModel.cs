using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ProviderProfilingRequestModel
    {
        public ProviderProfilingRequestModel()
        {
            AllocationValueByDistributionPeriod = Enumerable.Empty<AllocationPeriodValue>();
        }

        public string FundingStreamPeriod { get; set; }

        public IEnumerable<AllocationPeriodValue> AllocationValueByDistributionPeriod { get; set; }
    }
}
