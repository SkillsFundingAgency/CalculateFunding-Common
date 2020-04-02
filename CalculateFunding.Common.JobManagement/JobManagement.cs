using System;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Jobs;
using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Utility;
using Polly;
using Serilog;
using Serilog.Events;

namespace CalculateFunding.Common.JobManagement
{
    public class JobManagement : IJobManagement
    {
        private readonly IJobsApiClient _jobsApiClient;
        private readonly AsyncPolicy _jobsApiClientPolicy;
        private readonly ILogger _logger;
        
        public JobManagement(IJobsApiClient jobsApiClient,
            ILogger logger,
            IJobManagementResiliencePolicies jobManagementResiliencePolicies)
        {
            Guard.ArgumentNotNull(jobsApiClient, nameof(jobsApiClient));
            Guard.ArgumentNotNull(logger, nameof(logger));
            Guard.ArgumentNotNull(jobManagementResiliencePolicies, nameof(jobManagementResiliencePolicies));

            _jobsApiClient = jobsApiClient;
            _logger = logger;
            _jobsApiClientPolicy = jobManagementResiliencePolicies.JobsApiClient;
        }

        public async Task<JobViewModel> RetrieveJobAndCheckCanBeProcessed(string jobId)
        {
            ApiResponse<JobViewModel> response = await _jobsApiClientPolicy.ExecuteAsync(() => _jobsApiClient.GetJobById(jobId));

            if (response == null || response.Content == null)
            {
                string error = $"Could not find the job with id: '{jobId}'";

                _logger.Write(LogEventLevel.Error, error);
                throw new Exception(error);
            }

            JobViewModel job = response.Content;

            if (job.CompletionStatus.HasValue)
            {
                string error = $"Received job with id: '{jobId}' is already in a completed state with status {job.CompletionStatus.ToString()}";

                _logger.Write(LogEventLevel.Information, error);
                throw new Exception(error);
            }

            return job;
        }

        public async Task UpdateJobStatus(string jobId, int percentComplete = 0, bool? completedSuccessfully = null, string outcome = null)
        {
            JobLogUpdateModel jobLogUpdateModel = new JobLogUpdateModel
            {
                CompletedSuccessfully = completedSuccessfully,
                ItemsProcessed = percentComplete,
                Outcome = outcome
            };

            await UpdateJobStatus(jobId, jobLogUpdateModel);
        }

        public async Task UpdateJobStatus(string jobId, int totalItemsCount, int failedItemsCount, bool? completedSuccessfully = null, string outcome = null)
        {
            JobLogUpdateModel jobLogUpdateModel = new JobLogUpdateModel
            {
                CompletedSuccessfully = completedSuccessfully,
                ItemsProcessed = totalItemsCount,
                ItemsFailed = failedItemsCount,
                ItemsSucceeded = totalItemsCount - failedItemsCount,
                Outcome = outcome
            };

            await UpdateJobStatus(jobId, jobLogUpdateModel);
        }

        public async Task UpdateJobStatus(string jobId, JobLogUpdateModel jobLogUpdateModel)
        {
            ApiResponse<JobLog> jobLogResponse = await _jobsApiClientPolicy.ExecuteAsync(() => _jobsApiClient.AddJobLog(jobId, jobLogUpdateModel));

            if (jobLogResponse == null || jobLogResponse.Content == null)
            {
                _logger.Write(LogEventLevel.Error, $"Failed to add a job log for job id '{jobId}'");
            }
        }
    }
}
