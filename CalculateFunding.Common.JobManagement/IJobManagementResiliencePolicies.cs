using Polly;

namespace CalculateFunding.Common.JobManagement
{
    public interface IJobManagementResiliencePolicies
    {
        Policy JobsApiClient { get; set; }
    }
}