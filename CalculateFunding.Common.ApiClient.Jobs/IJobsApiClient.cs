using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.ApiClient.Models;

namespace CalculateFunding.Common.ApiClient.Jobs
{
    public interface IJobsApiClient
    {
        Task<ApiResponse<JobLog>> AddJobLog(string jobId, JobLogUpdateModel jobLogUpdateModel);

        Task<ApiResponse<JobViewModel>> GetJobById(string jobId);

        Task<ApiResponse<JobSummary>> GetLatestJobForSpecification(string specificationId, IEnumerable<string> jobTypes);

        Task<Job> CreateJob(JobCreateModel jobCreateModel);

        Task<IEnumerable<Job>> CreateJobs(IEnumerable<JobCreateModel> jobCreateModels);

        Task<ApiResponse<IEnumerable<JobDefinition>>> GetJobDefinitions();

        Task<ApiResponse<IEnumerable<JobSummary>>> GetNonCompletedJobsWithinTimeFrame(DateTimeOffset dateTimeFrom, DateTimeOffset dateTimeTo);
    }
}
