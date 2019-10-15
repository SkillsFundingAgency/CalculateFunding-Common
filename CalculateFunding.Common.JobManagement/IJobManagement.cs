using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Jobs.Models;

namespace CalculateFunding.Common.JobManagement
{
    public interface IJobManagement
    {
        Task<JobViewModel> RetrieveJobAndCheckCanBeProcessed(string jobId);
        Task UpdateJobStatus(string jobId, int percentComplete = 0, bool? completedSuccessfully = null, string outcome = null);
        Task UpdateJobStatus(string jobId, int totalItemsCount, int failedItemsCount, bool? completedSuccessfully = null, string outcome = null);
        Task UpdateJobStatus(string jobId, JobLogUpdateModel jobLogUpdateModel);
    }
}