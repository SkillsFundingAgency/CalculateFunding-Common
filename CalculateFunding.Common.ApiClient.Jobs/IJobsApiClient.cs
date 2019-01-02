using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.ApiClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Jobs
{
    public interface IJobsApiClient
    {
        Task<ApiResponse<JobLog>> AddJobLog(string jobId, JobLogUpdateModel jobLogUpdateModel);

        Task<ApiResponse<JobViewModel>> GetJobById(string jobId);

        Task<Job> CreateJob(JobCreateModel jobCreateModel);

        Task<IEnumerable<Job>> CreateJobs(IEnumerable<JobCreateModel> jobCreateModels);
    }
}
