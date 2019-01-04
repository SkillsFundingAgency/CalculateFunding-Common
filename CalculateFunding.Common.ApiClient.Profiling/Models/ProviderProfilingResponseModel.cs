using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ProviderProfilingResponseModel
    {
        public ProviderProfilingResponseModel()
        {
            DeliveryProfilePeriods = Enumerable.Empty<ProfilingPeriod>();
        }

        public ProviderProfilingRequestModel AllocationProfileRequest { get; set; }

        public IEnumerable<ProfilingPeriod> DeliveryProfilePeriods { get; set; }
    }
}
