using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Jobs.Models;

namespace CalculateFunding.Common.JobManagement
{
    public interface IJobManagement
    {
        Task<(bool Ok, string Message)> IsHealthOk(string queueName);
        Task<bool> QueueJobAndWait(Func<Task<bool>> queueJob, string jobType, string specificationId, string correlationId, string jobNotificationTopic, double pollTimeout = 600000, double pollInterval = 120000);
        Task<JobViewModel> RetrieveJobAndCheckCanBeProcessed(string jobId);
        Task UpdateJobStatus(string jobId, int percentComplete = 0, bool? completedSuccessfully = null, string outcome = null);
        Task UpdateJobStatus(string jobId, int totalItemsCount, int failedItemsCount, bool? completedSuccessfully = null, string outcome = null);
        Task UpdateJobStatus(string jobId, JobLogUpdateModel jobLogUpdateModel);
        Task<Job> QueueJob(JobCreateModel jobCreateModel);
        Task<IEnumerable<Job>> QueueJobs(IEnumerable<JobCreateModel> jobCreateModels);
        Task<IDictionary<string, JobSummary>> GetLatestJobsForSpecification(string specificationId, IEnumerable<string> jobTypes);
        Task<IDictionary<string, JobSummary>> GetLatestJobs(IEnumerable<string> jobTypes, string specificationId = null);
        Task<JobLog> AddJobLog(string jobId, JobLogUpdateModel jobLogUpdateModel);
        Task<IEnumerable<JobSummary>> GetNonCompletedJobsWithinTimeFrame(DateTimeOffset dateTimeFrom, DateTimeOffset dateTimeTo);

        Task<JobViewModel> GetJobById(string jobId);
        Task<JobCreateResult> TryQueueJob(JobCreateModel jobCreateModel);
        Task<int> GetJobsCountByJobDefinitionIdAndStatus(string specificationId, string jobDefinitionId, string runningStatus, string completionStatus);
    }
}