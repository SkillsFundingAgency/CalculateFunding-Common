using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ProviderProfilingResponseModel
    {
        public ProviderProfilingResponseModel()
        {
            DeliveryProfilePeriods = Enumerable.Empty<ProfilingPeriod>();
            DistributionPeriods = Enumerable.Empty<DistributionPeriods>();
        }

        public IEnumerable<ProfilingPeriod> DeliveryProfilePeriods { get; set; }

        public IEnumerable<DistributionPeriods> DistributionPeriods { get; }
    }
}
