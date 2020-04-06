using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
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

        public async Task<bool> WaitForJobsToComplete(IEnumerable<string> jobTypes, string specificationId, double pollTimeout = 600000, double pollInterval = 120000)
        {
            bool pollResult =  await Poll(async () => await CheckAllJobs(jobTypes, specificationId, _ => _ != null && (_.RunningStatus == RunningStatus.InProgress || _.RunningStatus == RunningStatus.Queued)),
                jobTypes,
                TimeSpan.FromMilliseconds(pollTimeout),
                TimeSpan.FromMilliseconds(pollInterval));

            if(!pollResult)
            {
                return false;
            }
            else
            {
                return await CheckAllJobs(jobTypes, specificationId, _ => _ == null || (_ != null && _.CompletionStatus != CompletionStatus.Failed));
            }
        }

        private async Task<bool> CheckAllJobs(IEnumerable<string> jobTypes, string specificationId, Predicate<JobSummary> predicate)
        {
            IEnumerable<Task<ApiResponse<JobSummary>>> jobResponses = jobTypes
                .Select(async _ => await _jobsApiClientPolicy.ExecuteAsync(() => {
                    return _jobsApiClient.GetLatestJobForSpecification(specificationId, new string[] { _ });
                }));

            IEnumerable<ApiResponse<JobSummary>> jobResponseSummaries = await Task.WhenAll(jobResponses.ToArray());

            if (jobResponseSummaries.All(_ => ((int?)_?.StatusCode >= 200) && ((int?)_?.StatusCode <= 299)))
            {
                IEnumerable<JobSummary> summaries = jobResponseSummaries.Select(_ => _.Content);

                return summaries.All(_ => predicate(_));
            }
            else
            {
                // any failures retrieving jobsummaries we ignore and keep polling as we don't know what state the jobs are in
                return true;
            }
        }

        private async Task<bool> Poll(Func<Task<bool>> condition, IEnumerable<string> jobTypes, TimeSpan timeout, TimeSpan delay)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource pollCancellationTokenSource = new CancellationTokenSource();

            try
            {
                _ = Task.Factory.StartNew(() =>
                {
                    if (!pollCancellationTokenSource.Token.WaitHandle.WaitOne(timeout))
                    {
                        _logger.Error($"Poll timeout waiting for the following job types : {jobTypes?.Aggregate((partialLog, log) => $"{partialLog}, {log}")} to complete.");
                        cancellationTokenSource.Cancel();
                    }
                });

                while ((await condition()))
                {
                    Thread.Sleep(delay);
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        break;
                }

                return !cancellationTokenSource.Token.IsCancellationRequested;
            }
            finally 
            {
                // make sure we cancel the poll timeout task
                pollCancellationTokenSource.Cancel();
            }
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
