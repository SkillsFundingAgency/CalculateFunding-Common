using Polly;

namespace CalculateFunding.Common.JobManagement
{
    public interface IJobManagementResiliencePolicies
    {
        AsyncPolicy JobsApiClient { get; set; }
    }
}