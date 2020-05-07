﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Jobs;
using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ServiceBus.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Polly;
using Serilog;
using Serilog.Events;

namespace CalculateFunding.Common.JobManagement.UnitTests
{
    [TestClass]
    public class JobManagementTests
    {
#if NCRUNCH
[Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(RetrieveJobAndCheckCanBeProcessedFailsTestCases), DynamicDataSourceType.Method)]
        public async Task RetrieveJobAndCheckCanBeProcessed_Fails_LogsAndErrors(ApiResponse<JobViewModel> jobApiResponse,
            string jobId,
            string errorMessage,
            LogEventLevel logEventLevel)
        {
            //Arrange
            var jobsApiClient = Substitute.For<IJobsApiClient>();
            var messengerService = Substitute.For<IMessengerService>();
            var policies = new JobManagementResiliencePolicies { JobsApiClient = Policy.NoOpAsync() };
            var logger = Substitute.For<ILogger>();

            jobsApiClient
                .GetJobById(Arg.Any<string>())
                .Returns(jobApiResponse);

            var jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            Func<Task> test = async () => await jobManagement.RetrieveJobAndCheckCanBeProcessed(jobId);

            test
                .Should().Throw<Exception>()
                .Which
                .Message
                .Should().Be(errorMessage);

            await jobsApiClient
                .Received(1)
                .GetJobById(jobId);

            logger
                .Received(1)
                .Write(logEventLevel, errorMessage);
        }

        private static IEnumerable<object[]> RetrieveJobAndCheckCanBeProcessedFailsTestCases()
        {
            string jobId = "3456";

            foreach (var response in new[] { new ApiResponse<JobViewModel>(HttpStatusCode.NotFound), null })
            {
                yield return new object[] { response, jobId, $"Could not find the job with id: '{jobId}'", LogEventLevel.Error };
            }

            foreach (var cs in new[]
            {
                CompletionStatus.Cancelled,
                CompletionStatus.Failed,
                CompletionStatus.Succeeded,
                CompletionStatus.Superseded,
                CompletionStatus.TimedOut
            })
            {
                yield return new object[]
                {
                    new ApiResponse<JobViewModel>(HttpStatusCode.OK, new JobViewModel { CompletionStatus = cs }),
                    jobId,
                    $"Received job with id: '{jobId}' is already in a completed state with status {cs.ToString()}",
                    LogEventLevel.Information
                };
            }
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task WaitForJobsToCompleteWithAllJobsSucceeded_ReturnsTrue(bool useServiceBus)
        {
            var jobsApiClient = Substitute.For<IJobsApiClient>();
            var policies = new JobManagementResiliencePolicies { JobsApiClient = Policy.NoOpAsync() };
            IMessengerService messengerService = null;

            if (useServiceBus)
            {
                messengerService = Substitute.For<IMessengerService, IServiceBusService>();

            }
            else
            {
                messengerService = Substitute.For<IMessengerService, IQueueService>();

            }

            var logger = Substitute.For<ILogger>();

            var jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            var jobId = "3456";

            jobsApiClient
                .GetLatestJobForSpecification("specificationId", Arg.Is<IEnumerable<string>>(_ => _.Single() == "PopulateScopedProviders"))
                .Returns(new ApiResponse<JobSummary>(HttpStatusCode.OK, new JobSummary { RunningStatus = RunningStatus.Completed, CompletionStatus = CompletionStatus.Succeeded, JobId = jobId }));

            messengerService
                .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobNotification>>(), TimeSpan.FromMilliseconds(600000))
                .Returns(new JobNotification { CompletionStatus = CompletionStatus.Succeeded });

            //Act
            bool jobsComplete = await jobManagement.QueueJobAndWait(async() =>  await Task.Run(() => { return true; }), "PopulateScopedProviders", "specificationId", "correlationId", "topic");

            //Assert
            if (useServiceBus)
            {
                await messengerService
                    .Received(1)
                    .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobNotification>>(), TimeSpan.FromMilliseconds(600000));
            }
            else
            {
                await jobsApiClient
                    .Received(2)
                    .GetLatestJobForSpecification("specificationId", Arg.Is<IEnumerable<string>>(_ => _.Single() == "PopulateScopedProviders"));
            }

            jobsComplete
                .Should()
                .BeTrue();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task WaitForJobsToCompleteWithNoJobsSucceeded_ReturnsFalse(bool useServiceBus)
        {
            var jobsApiClient = Substitute.For<IJobsApiClient>();
            var policies = new JobManagementResiliencePolicies { JobsApiClient = Policy.NoOpAsync() };

            IMessengerService messengerService = null;

            if (useServiceBus)
            {
                messengerService = Substitute.For<IMessengerService, IServiceBusService>();

            }
            else
            {
                messengerService = Substitute.For<IMessengerService, IQueueService>();
            }

            var logger = Substitute.For<ILogger>();

            var jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            var jobId = "3456";

            jobsApiClient
                .GetLatestJobForSpecification("specificationId", Arg.Is<IEnumerable<string>>(_ => _.Single() == "PopulateScopedProviders"))
                .Returns(new ApiResponse<JobSummary>(HttpStatusCode.OK, new JobSummary { RunningStatus = RunningStatus.InProgress, JobId = jobId }));

            messengerService
                .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobNotification>>(), TimeSpan.FromMilliseconds(1000))
                .Returns(default(JobNotification));

            //Act
            bool jobsComplete = await jobManagement.QueueJobAndWait(async () => await Task.Run(() => { return true; }), "PopulateScopedProviders", "specificationId", "correlationId", "topic", 1000, 100);

            //Assert
            if (useServiceBus)
            {
                await messengerService
                    .Received(1)
                    .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobNotification>>(), TimeSpan.FromMilliseconds(1000));
            }

            jobsComplete
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public async Task WaitForJobsToCompleteWithNoJobsRunning_ReturnsTrue()
        {
            var jobsApiClient = Substitute.For<IJobsApiClient>();
            var policies = new JobManagementResiliencePolicies { JobsApiClient = Policy.NoOpAsync() };
            var messengerService = Substitute.For<IMessengerService>(); 
            var logger = Substitute.For<ILogger>();

            var jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            jobsApiClient
                .GetLatestJobForSpecification("specificationId", Arg.Is<IEnumerable<string>>(_ => _.Single() == "PopulateScopedProviders"))
                .Returns(new ApiResponse<JobSummary>(HttpStatusCode.OK, null));

            //Act
            bool jobsComplete = await jobManagement.QueueJobAndWait(async () => await Task.Run(() => { return true; }), "PopulateScopedProviders", "specificationId", "correlationId", "topic");
            
            //Assert
            jobsComplete
                .Should()
                .BeTrue();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task WaitForJobsToCompleteWithJobsFailed_ReturnsFalse(bool useServiceBus)
        {
            var jobsApiClient = Substitute.For<IJobsApiClient>();
            var policies = new JobManagementResiliencePolicies { JobsApiClient = Policy.NoOpAsync() };

            IMessengerService messengerService = null;

            if (useServiceBus)
            {
                messengerService = Substitute.For<IMessengerService, IServiceBusService>();

            }
            else
            {
                messengerService = Substitute.For<IMessengerService, IQueueService>();
            }

            var logger = Substitute.For<ILogger>();

            var jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            var jobId = "3456";

            jobsApiClient
                .GetLatestJobForSpecification("specificationId", Arg.Is<IEnumerable<string>>(_ => _.Single() == "PopulateScopedProviders"))
                .Returns(new ApiResponse<JobSummary>(HttpStatusCode.OK, new JobSummary { RunningStatus = RunningStatus.Completed, CompletionStatus = CompletionStatus.Failed, JobId = jobId }));

            messengerService
                .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobNotification>>(), TimeSpan.FromMilliseconds(600000))
                .Returns(new JobNotification { CompletionStatus = CompletionStatus.Failed });

            //Act
            bool jobsComplete = await jobManagement.QueueJobAndWait(async () => await Task.Run(() => { return true; }), "PopulateScopedProviders", "specificationId", "correlationId", "topic");

            //Assert
            if (useServiceBus)
            {
                await messengerService
                    .Received(1)
                    .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobNotification>>(), TimeSpan.FromMilliseconds(600000));
            }

            jobsComplete
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public async Task RetrieveJobAndCheckCanBeProcessed_ApiReturnsIncomplete_ReturnsCorrectly()
        {
            //Arrange
            var jobsApiClient = Substitute.For<IJobsApiClient>();
            var policies = new JobManagementResiliencePolicies { JobsApiClient = Policy.NoOpAsync() };
            var messengerService = Substitute.For<IMessengerService>();
            var logger = Substitute.For<ILogger>();

            var jvm = new JobViewModel { CompletionStatus = null };

            var jobApiResponse = new ApiResponse<JobViewModel>(HttpStatusCode.OK, jvm);

            jobsApiClient
                .GetJobById(Arg.Any<string>())
                .Returns(jobApiResponse);

            var jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            var jobId = "3456";

            //Act
            var viewModel = await jobManagement.RetrieveJobAndCheckCanBeProcessed(jobId);

            //Assert
            await jobsApiClient
                .Received(1)
                .GetJobById(jobId);

            viewModel
                .Should()
                .Be(jvm);
        }

#if NCRUNCH
[Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(UpdateJobStatusApiResponseFailureTestCases), DynamicDataSourceType.Method)]
        public async Task UpdateJobStatusInternal_ApiResponseFailure_Logs(ApiResponse<JobLog> jobLogApiResponse)
        {
            //Arrange
            var jobsApiClient = Substitute.For<IJobsApiClient>();
            var policies = new JobManagementResiliencePolicies { JobsApiClient = Policy.NoOpAsync() };
            var messengerService = Substitute.For<IMessengerService>();
            var logger = Substitute.For<ILogger>();

            jobsApiClient
                .AddJobLog(Arg.Any<string>(), Arg.Any<JobLogUpdateModel>())
                .Returns(jobLogApiResponse);

            var updateModel = new JobLogUpdateModel();

            var jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            var jobId = "3456";

            //Act
            await jobManagement.UpdateJobStatus(jobId, updateModel);

            //Assert
            await jobsApiClient
                .Received(1)
                .AddJobLog(jobId, updateModel);

            logger
                .Received(1)
                .Write(LogEventLevel.Error, $"Failed to add a job log for job id '{jobId}'");
        }

        private static IEnumerable<object[]> UpdateJobStatusApiResponseFailureTestCases()
        {
            yield return new object[] { new ApiResponse<JobLog>(HttpStatusCode.NotFound) };
            yield return new object[] { null };
        }

        [TestMethod]
        public async Task UpdateJobStatus_ApiResponseSuccess_Runs()
        {
            //Arrange
            var jobsApiClient = Substitute.For<IJobsApiClient>();
            var policies = new JobManagementResiliencePolicies { JobsApiClient = Policy.NoOpAsync() };
            var messengerService = Substitute.For<IMessengerService>();
            var logger = Substitute.For<ILogger>();

            var jobLogApiResponse = new ApiResponse<JobLog>(HttpStatusCode.OK, new JobLog());

            jobsApiClient
                .AddJobLog(Arg.Any<string>(), Arg.Any<JobLogUpdateModel>())
                .Returns(jobLogApiResponse);

            var updateModel = new JobLogUpdateModel();

            var jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            var jobId = "3456";

            //Act
            await jobManagement.UpdateJobStatus(jobId, updateModel);

            //Assert
            await jobsApiClient
                .Received(1)
                .AddJobLog(jobId, updateModel);

            logger
                .Received(0)
                .Write(Arg.Any<LogEventLevel>(), Arg.Any<string>());
        }
    }
}
