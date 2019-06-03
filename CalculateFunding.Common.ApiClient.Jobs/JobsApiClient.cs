using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Jobs
{
    public class JobsApiClient : BaseApiClient, IJobsApiClient
    {
        public JobsApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
          : base(httpClientFactory, HttpClientKeys.Jobs, logger, cancellationTokenProvider)
        { }

        public async Task<ApiResponse<JobLog>> AddJobLog(string jobId, JobLogUpdateModel jobLogUpdateModel)
        {
            Guard.IsNullOrWhiteSpace(jobId, nameof(jobId));
            Guard.ArgumentNotNull(jobLogUpdateModel, nameof(jobLogUpdateModel));

            string url = $"jobs/{jobId}/logs";

            return await PostAsync<JobLog, JobLogUpdateModel>(url, jobLogUpdateModel);
        }

        public async Task<ApiResponse<JobViewModel>> GetJobById(string jobId)
        {
            Guard.IsNullOrWhiteSpace(jobId, nameof(jobId));

            string url = $"jobs/{jobId}";

            return await GetAsync<JobViewModel>(url);
        }

        public async Task<ApiResponse<JobSummary>> GetLatestJobForSpecification(string specificationId, IEnumerable<string> jobTypes)
        {
            Guard.ArgumentNotNull(specificationId, nameof(specificationId));

            string api = $"latest?specificationId={specificationId}";

            if (jobTypes != null && jobTypes.Count() > 0)
            {
                api += $"&jobTypes={string.Join(",", jobTypes)}";
            }

            return await GetAsync<JobSummary>(api);
        }

        public async Task<Job> CreateJob(JobCreateModel jobCreateModel)
        {
            Guard.ArgumentNotNull(jobCreateModel, nameof(jobCreateModel));

            string url = $"jobs";

            ApiResponse<IEnumerable<Job>> jobs = await PostAsync<IEnumerable<Job>, IEnumerable<JobCreateModel>>(url, new[] { jobCreateModel });

            if (jobs.Content.IsNullOrEmpty())
            {
                throw new Exception($"Failed to create new job of type {jobCreateModel.JobDefinitionId}");
            }

            return jobs.Content.First();
        }

        public async Task<IEnumerable<Job>> CreateJobs(IEnumerable<JobCreateModel> jobCreateModels)
        {
            Guard.ArgumentNotNull(jobCreateModels, nameof(jobCreateModels));

            string url = $"jobs";

            ApiResponse<IEnumerable<Job>> jobs = await PostAsync<IEnumerable<Job>, IEnumerable<JobCreateModel>>(url, jobCreateModels);

            if (jobs.Content.IsNullOrEmpty())
            {
                throw new Exception($"Failed to create jobs");
            }

            return jobs.Content;
        }

        public async Task<ApiResponse<IEnumerable<JobDefinition>>> GetJobDefinitions()
        {
            string url = "jobdefinitions";

            return await GetAsync<IEnumerable<JobDefinition>>(url);
        }

        public async Task<ApiResponse<IEnumerable<JobSummary>>> GetNonCompletedJobsWithinTimeFrame(DateTimeOffset dateTimeFrom, DateTimeOffset dateTimeTo)
        {
            const string dateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

            string dateTimeFromAsString = dateTimeFrom.ToUniversalTime().ToString(dateFormat);
            string dateTimeToAsString = dateTimeTo.ToUniversalTime().ToString(dateFormat);

            string url = $"jobs/noncompleted/dateTimeFrom/{dateTimeFromAsString}/dateTimeTo/{dateTimeToAsString}";

            return await GetAsync<IEnumerable<JobSummary>>(url);
        }
    }
}
