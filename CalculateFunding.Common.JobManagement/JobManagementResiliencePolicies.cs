using Polly;

namespace CalculateFunding.Common.JobManagement
{
    public class JobManagementResiliencePolicies : IJobManagementResiliencePolicies
    {
        public AsyncPolicy JobsApiClient { get; set; }
    }
}
