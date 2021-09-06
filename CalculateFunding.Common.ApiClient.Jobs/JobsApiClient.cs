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
        public JobsApiClient(IHttpClientFactory httpClientFactory,
            ILogger logger,
            ICancellationTokenProvider cancellationTokenProvider = null, string clientKey = null)
            : base(httpClientFactory, clientKey ?? HttpClientKeys.Jobs, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<JobLog>> AddJobLog(string jobId,
            JobLogUpdateModel jobLogUpdateModel)
        {
            Guard.IsNullOrWhiteSpace(jobId, nameof(jobId));
            Guard.ArgumentNotNull(jobLogUpdateModel, nameof(jobLogUpdateModel));

            return await PostAsync<JobLog, JobLogUpdateModel>($"jobs/{jobId}/logs", jobLogUpdateModel);
        }

        public async Task<ApiResponse<JobViewModel>> GetJobById(string jobId)
        {
            Guard.IsNullOrWhiteSpace(jobId, nameof(jobId));

            return await GetAsync<JobViewModel>($"jobs/{jobId}");
        }

        public async Task<ApiResponse<IDictionary<string, JobSummary>>> GetLatestJobsForSpecification(string specificationId, params string[] jobDefinitionIds)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string api = $"jobs/latest?specificationId={specificationId}";

            if (jobDefinitionIds?.Any() == true)
            {
                foreach (string jobDefinitionId in jobDefinitionIds)
                {
                    api += $"&jobDefinitionIds={jobDefinitionId}";
                }
            }

            return await GetAsync<IDictionary<string, JobSummary>>(api);
        }

        public async Task<ApiResponse<IDictionary<string, JobSummary>>> GetLatestJobsByJobDefinitionIds(params string[] jobDefinitionIds)
        {
            string api = $"jobs/latest-by-job-definition-ids";

            if (jobDefinitionIds?.Any() == true)
            {
                foreach (string jobDefinitionId in jobDefinitionIds)
                {
                    api += $"&jobDefinitionIds={jobDefinitionId}";
                }
            }

            return await GetAsync<IDictionary<string, JobSummary>>(api);
        }

        public async Task<Job> CreateJob(JobCreateModel jobCreateModel)
        {
            Guard.ArgumentNotNull(jobCreateModel, nameof(jobCreateModel));

            string url = "jobs";

            ApiResponse<IEnumerable<Job>> jobs = await PostAsync<IEnumerable<Job>, IEnumerable<JobCreateModel>>(url,
                new[]
                {
                    jobCreateModel
                });

            if (jobs.Content.IsNullOrEmpty())
            {
                throw new Exception($"Failed to create new job of type {jobCreateModel.JobDefinitionId}");
            }

            return jobs.Content.First();
        }

        public async Task<ApiResponse<JobCreateResult>> TryCreateJob(JobCreateModel jobCreateModel)
        {
            Guard.ArgumentNotNull(jobCreateModel, nameof(jobCreateModel));

            return await PostAsync<JobCreateResult, JobCreateModel>("jobs/try-create-job",
                jobCreateModel);
        }

        public async Task<ApiResponse<IEnumerable<JobCreateResult>>> TryCreateJobs(IEnumerable<JobCreateModel> jobCreateModels)
        {
            Guard.IsNotEmpty(jobCreateModels, nameof(jobCreateModels));

            return await PostAsync<IEnumerable<JobCreateResult>, IEnumerable<JobCreateModel>>("jobs/try-create-jobs",
                jobCreateModels);
        }

        public async Task<IEnumerable<Job>> CreateJobs(IEnumerable<JobCreateModel> jobCreateModels)
        {
            Guard.IsNotEmpty(jobCreateModels, nameof(jobCreateModels));

            ApiResponse<IEnumerable<Job>> jobs = await PostAsync<IEnumerable<Job>, IEnumerable<JobCreateModel>>("jobs", jobCreateModels);

            if (jobs.Content.IsNullOrEmpty())
            {
                throw new Exception("Failed to create jobs");
            }

            return jobs.Content;
        }

        public async Task<ApiResponse<IEnumerable<JobDefinition>>> GetJobDefinitions()
        {
            return await GetAsync<IEnumerable<JobDefinition>>("jobs/jobdefinitions");
        }

        public async Task<ApiResponse<JobDefinition>> GetJobDefinition(string jobDefinitionId)
        {
            Guard.IsNullOrWhiteSpace(jobDefinitionId, nameof(jobDefinitionId));

            return await GetAsync<JobDefinition>($"jobs/jobdefinitions/{jobDefinitionId}");
        }

        public async Task<ApiResponse<IEnumerable<JobSummary>>> GetNonCompletedJobsWithinTimeFrame(DateTimeOffset dateTimeFrom,
            DateTimeOffset dateTimeTo)
        {
            const string dateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";

            string dateTimeFromAsString = dateTimeFrom.ToUniversalTime().ToString(dateFormat);
            string dateTimeToAsString = dateTimeTo.ToUniversalTime().ToString(dateFormat);

            return await GetAsync<IEnumerable<JobSummary>>($"jobs/noncompleted/dateTimeFrom/{dateTimeFromAsString}/dateTimeTo/{dateTimeToAsString}");
        }

        public async Task<ApiResponse<JobSummary>> GetLatestSuccessfulJobForSpecification(string specificationId, string jobDefinitionId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string api = $"jobs/latest-success?specificationId={specificationId}&jobDefinitionId={jobDefinitionId}";

            return await GetAsync<JobSummary>(api);
        }

        public async Task<ApiResponse<JobSummary>> GetLatestJobByTriggerEntityId(string specificationId, string entityId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(entityId, nameof(entityId));

            string api = $"jobs/latest-by-entity-id?specificationId={specificationId}&entityId={entityId}";

            return await GetAsync<JobSummary>(api);
        }
    }
}