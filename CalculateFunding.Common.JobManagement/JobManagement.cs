using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Jobs;
using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ServiceBus.Interfaces;
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
        private readonly IMessengerService _messengerService;

        private bool IsServiceBusService => _messengerService.GetType().GetInterfaces().Contains(typeof(IServiceBusService));

        public JobManagement(IJobsApiClient jobsApiClient,
            ILogger logger,
            IJobManagementResiliencePolicies jobManagementResiliencePolicies,
            IMessengerService messengerService)
        {
            Guard.ArgumentNotNull(jobsApiClient, nameof(jobsApiClient));
            Guard.ArgumentNotNull(logger, nameof(logger));
            Guard.ArgumentNotNull(jobManagementResiliencePolicies, nameof(jobManagementResiliencePolicies));
            Guard.ArgumentNotNull(messengerService, nameof(messengerService));

            _jobsApiClient = jobsApiClient;
            _logger = logger;
            _jobsApiClientPolicy = jobManagementResiliencePolicies.JobsApiClient;
            _messengerService = messengerService;
        }

        public async Task<(bool Ok, string Message)> IsHealthOk(string queueName)
        {
            return await _messengerService.IsHealthOk(queueName);
        }

        public async Task<bool> QueueJobAndWait(Func<Task<bool>> queueJob ,string jobType, string specificationId, string correlationId, string jobNotificationTopic, double pollTimeout = 600000, double pollInterval = 120000)
        {
            if (IsServiceBusService)
            {
                await ((IServiceBusService)_messengerService).CreateSubscription(jobNotificationTopic, correlationId);
            }

            bool jobQueued = await queueJob();

            if (jobQueued)
            {
                try
                {
                    if (IsServiceBusService)
                    {
                        JobNotification scopedJob = await _messengerService.ReceiveMessage<JobNotification>($"{jobNotificationTopic}/Subscriptions/{correlationId}", _ =>
                        {
                            return _?.JobType == jobType &&
                            _.SpecificationId == specificationId &&
                            (_.CompletionStatus == CompletionStatus.Succeeded || _.CompletionStatus == CompletionStatus.Failed);
                        },
                        TimeSpan.FromMilliseconds(pollTimeout));

                        return scopedJob?.CompletionStatus == CompletionStatus.Succeeded;
                    }
                    else
                    {
                        return await WaitForJobToComplete(jobType, specificationId, pollTimeout, pollInterval);
                    }
                }
                finally
                {
                    if (IsServiceBusService)
                    {
                        await ((IServiceBusService)_messengerService).DeleteSubscription(jobNotificationTopic, correlationId);
                    }
                }
            }
            else
            {
                // if job not queued then return true
                return true;
            }
        }

        private async Task<bool> WaitForJobToComplete(string jobType, string specificationId, double pollTimeout, double pollInterval)
        {
            bool pollResult = await Poll(async () => await CheckAllJobs(jobType, specificationId, _ => _ != null && (_.RunningStatus == RunningStatus.InProgress || _.RunningStatus == RunningStatus.Queued)),
                    jobType,
                    TimeSpan.FromMilliseconds(pollTimeout),
                    TimeSpan.FromMilliseconds(pollInterval));

            if (!pollResult)
            {
                return false;
            }
            else
            {
                return await CheckAllJobs(jobType, specificationId, _ => _ == null || (_ != null && _.CompletionStatus != CompletionStatus.Failed));
            }
        }

        private async Task<bool> CheckAllJobs(string jobType, string specificationId, Predicate<JobSummary> predicate)
        {
            ApiResponse<JobSummary> jobResponse = await _jobsApiClientPolicy.ExecuteAsync(() => {
                    return _jobsApiClient.GetLatestJobForSpecification(specificationId, new string[] { jobType });
                });

            if ((int?)jobResponse?.StatusCode >= 200 && (int?)jobResponse?.StatusCode <= 299)
            {
                JobSummary summary = jobResponse.Content;

                return predicate(summary);
            }
            else
            {
                // any failures retrieving jobsummaries we ignore and keep polling as we don't know what state the jobs are in
                return true;
            }
        }

        private async Task<bool> Poll(Func<Task<bool>> condition, string jobType, TimeSpan timeout, TimeSpan delay)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationTokenSource pollCancellationTokenSource = new CancellationTokenSource();

            try
            {
                _ = Task.Factory.StartNew(() =>
                {
                    if (!pollCancellationTokenSource.Token.WaitHandle.WaitOne(timeout))
                    {
                        _logger.Error($"Poll timeout waiting for the following job type : {jobType} to complete.");
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
                throw new JobNotFoundException(error, jobId);
            }

            JobViewModel job = response.Content;

            if (job.CompletionStatus.HasValue)
            {
                string error = $"Received job with id: '{jobId}' is already in a completed state with status {job.CompletionStatus}";

                _logger.Write(LogEventLevel.Information, error);
                throw new JobAlreadyCompletedException(error, job);
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

        public async Task<Job> QueueJob(JobCreateModel jobCreateModel)
        {
            return await _jobsApiClientPolicy.ExecuteAsync(() => {
                return _jobsApiClient.CreateJob(jobCreateModel);
            });
        }

        public async Task<IEnumerable<Job>> QueueJobs(IEnumerable<JobCreateModel> jobCreateModels)
        {
            return await _jobsApiClientPolicy.ExecuteAsync(() => {
                return _jobsApiClient.CreateJobs(jobCreateModels);
            });
        }

        public async Task<JobSummary> GetLatestJobForSpecification(string specificationId, IEnumerable<string> jobTypes)
        {
            ApiResponse<JobSummary> jobSummaryResponse = await _jobsApiClientPolicy.ExecuteAsync(() => _jobsApiClient.GetLatestJobForSpecification(specificationId, jobTypes));

            JobSummary jobSummary = jobSummaryResponse?.Content;

            return jobSummary;
        }

        public async Task<JobLog> AddJobLog(string jobId, JobLogUpdateModel jobLogUpdateModel)
        {
            ApiResponse<JobLog> jobLogResponse = await _jobsApiClientPolicy.ExecuteAsync(() => _jobsApiClient.AddJobLog(jobId, jobLogUpdateModel));

            JobLog jobLog = jobLogResponse?.Content;

            return jobLog;
        }

        public async Task<IEnumerable<JobSummary>> GetNonCompletedJobsWithinTimeFrame(DateTimeOffset dateTimeFrom, DateTimeOffset dateTimeTo)
        {
            ApiResponse<IEnumerable<JobSummary>> jobSummaryResponse = await _jobsApiClientPolicy.ExecuteAsync(() => _jobsApiClient.GetNonCompletedJobsWithinTimeFrame(dateTimeFrom, dateTimeTo));

            IEnumerable<JobSummary> jobSummaries = jobSummaryResponse?.Content;

            return jobSummaries;
        }

        public async Task<JobViewModel> GetJobById(string jobId)
        {
            ApiResponse<JobViewModel> jobResponse = await _jobsApiClientPolicy.ExecuteAsync(() => _jobsApiClient.GetJobById(jobId));

            JobViewModel jobViewModel = jobResponse?.Content;

            return jobViewModel;
        }
    }
}
