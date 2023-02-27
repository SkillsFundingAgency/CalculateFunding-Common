using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ProviderProfilingResponseModel
    {
        public IEnumerable<ProfilingPeriod> DeliveryProfilePeriods { get; set; }

        public IEnumerable<DistributionPeriods> DistributionPeriods { get; set; }

        public string ProfilePatternKey { get; set; }

        public string ProfilePatternDisplayName { get; set; }
        public ProfilePatternType ProfilePatternType { get; set; }
    }
}
