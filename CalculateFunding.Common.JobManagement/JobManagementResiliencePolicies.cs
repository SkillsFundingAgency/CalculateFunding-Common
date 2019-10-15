using Polly;

namespace CalculateFunding.Common.JobManagement
{
    public class JobManagementResiliencePolicies : IJobManagementResiliencePolicies
    {
        public Policy JobsApiClient { get; set; }
    }
}
