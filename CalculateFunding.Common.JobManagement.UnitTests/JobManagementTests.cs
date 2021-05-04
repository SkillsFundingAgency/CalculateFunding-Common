using System;
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
using Serilog.Core;
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
        [DynamicData(nameof(RetrieveJobAndCheckCanBeProcessedFailsTestCasesWithJobNotFound), DynamicDataSourceType.Method)]
        public async Task RetrieveJobAndCheckCanBeProcessed_FailsWithJobNotFound_LogsAndErrors(ApiResponse<JobViewModel> jobApiResponse,
            string jobId,
            string errorMessage,
            LogEventLevel logEventLevel)
        {
            //Arrange
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            ILogger logger = Substitute.For<ILogger>();

            jobsApiClient
                .GetJobById(Arg.Any<string>())
                .Returns(jobApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            Func<Task> test = async () => await jobManagement.RetrieveJobAndCheckCanBeProcessed(jobId);

            test
                .Should().Throw<JobNotFoundException>()
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

        [TestMethod]
        [DynamicData(nameof(RetrieveJobAndCheckCanBeProcessedFailsTestCasesWithJobAlreadyCompleted), DynamicDataSourceType.Method)]
        public async Task RetrieveJobAndCheckCanBeProcessed_FailsWithJobAlreadyCompleted_LogsAndErrors(ApiResponse<JobViewModel> jobApiResponse,
            string jobId,
            string errorMessage,
            LogEventLevel logEventLevel)
        {
            //Arrange
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            ILogger logger = Substitute.For<ILogger>();

            jobsApiClient
                .GetJobById(Arg.Any<string>())
                .Returns(jobApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            Func<Task> test = async () => await jobManagement.RetrieveJobAndCheckCanBeProcessed(jobId);

            test
                .Should().Throw<JobAlreadyCompletedException>()
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


        private static IEnumerable<object[]> RetrieveJobAndCheckCanBeProcessedFailsTestCasesWithJobNotFound()
        {
            string jobId = "3456";

            foreach (ApiResponse<JobViewModel> response in new[]
            {
                new ApiResponse<JobViewModel>(HttpStatusCode.NotFound), null
            })
            {
                yield return new object[]
                {
                    response, jobId, $"Could not find the job with id: '{jobId}'", LogEventLevel.Error
                };
            }
        }

        private static IEnumerable<object[]> RetrieveJobAndCheckCanBeProcessedFailsTestCasesWithJobAlreadyCompleted()
        {
            string jobId = "3456";

            foreach (CompletionStatus cs in new[]
            {
                CompletionStatus.Cancelled, CompletionStatus.Failed, CompletionStatus.Succeeded, CompletionStatus.Superseded, CompletionStatus.TimedOut
            })
            {
                yield return new object[]
                {
                    new ApiResponse<JobViewModel>(HttpStatusCode.OK,
                        new JobViewModel
                        {
                            CompletionStatus = cs
                        }),
                    jobId, $"Received job with id: '{jobId}' is already in a completed state with status {cs}", LogEventLevel.Information
                };
            }
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task WaitForJobsToCompleteWithAllJobsSucceeded_ReturnsTrue(bool useServiceBus)
        {
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = null;

            if (useServiceBus)
            {
                messengerService = Substitute.For<IMessengerService, IServiceBusService>();
            }
            else
            {
                messengerService = Substitute.For<IMessengerService, IQueueService>();
            }

            ILogger logger = Substitute.For<ILogger>();

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            var jobId = "3456";
            IEnumerable<Job> jobApiResponse = new List<Job> { new Job { Id = jobId } };
            jobsApiClient
                .GetLatestJobsForSpecification("specificationId", Arg.Is<string[]>(_ => _.Single() == "PopulateScopedProviders"))
                .Returns(new ApiResponse<IDictionary<string, JobSummary>>(HttpStatusCode.OK, new Dictionary<string, JobSummary> { { string.Empty, new JobSummary { RunningStatus = RunningStatus.Completed, CompletionStatus = CompletionStatus.Succeeded, JobId = jobId } } }));

            messengerService
                .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobSummary>>(), TimeSpan.FromMilliseconds(600000))
                .Returns(new JobSummary
                {
                    CompletionStatus = CompletionStatus.Succeeded
                });

            //Act
            bool jobsComplete = await jobManagement.QueueJobAndWait(async () => await Task.Run(() => { return true; }), "PopulateScopedProviders", "specificationId", "correlationId", "topic");

            //Assert
            if (useServiceBus)
            {
                await ((IServiceBusService)messengerService)
                    .Received(1)
                    .CreateSubscription("topic", "correlationId", Arg.Is<TimeSpan>(_ => _.Days == 1));

                await messengerService
                    .Received(1)
                    .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobSummary>>(), TimeSpan.FromMilliseconds(600000));
            }
            else
            {
                await jobsApiClient
                    .Received(2)
                    .GetLatestJobsForSpecification("specificationId", Arg.Is<string[]>(_ => _.Single() == "PopulateScopedProviders"));
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
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };

            IMessengerService messengerService = null;

            if (useServiceBus)
            {
                messengerService = Substitute.For<IMessengerService, IServiceBusService>();
            }
            else
            {
                messengerService = Substitute.For<IMessengerService, IQueueService>();
            }

            ILogger logger = Substitute.For<ILogger>();

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            string jobId = "3456";

            jobsApiClient
                .GetLatestJobsForSpecification("specificationId", Arg.Is<string[]>(_ => _.Single() == "PopulateScopedProviders"))
                .Returns(new ApiResponse<IDictionary<string, JobSummary>>(HttpStatusCode.OK, new Dictionary<string, JobSummary> { { string.Empty, new JobSummary { RunningStatus = RunningStatus.InProgress, JobId = jobId } } }));

            messengerService
                .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobSummary>>(), TimeSpan.FromMilliseconds(1000))
                .Returns(default(JobSummary));

            //Act
            bool jobsComplete = await jobManagement.QueueJobAndWait(async () => await Task.Run(() => { return true; }),
                "PopulateScopedProviders",
                "specificationId",
                "correlationId",
                "topic",
                1000,
                100);

            //Assert
            if (useServiceBus)
            {
                await ((IServiceBusService)messengerService)
                       .Received(1)
                       .CreateSubscription("topic", "correlationId", Arg.Is<TimeSpan>(_ => _.Days == 1));

                await messengerService
                    .Received(1)
                    .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobSummary>>(), TimeSpan.FromMilliseconds(1000));
            }

            jobsComplete
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public async Task WaitForJobsToCompleteWithNoJobsRunning_ReturnsTrue()
        {
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            jobsApiClient
                .GetLatestJobsForSpecification("specificationId", Arg.Is<string[]>(_ => _.Single() == "PopulateScopedProviders"))
                .Returns(new ApiResponse<IDictionary<string, JobSummary>>(HttpStatusCode.NoContent));

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
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };

            IMessengerService messengerService = null;

            if (useServiceBus)
            {
                messengerService = Substitute.For<IMessengerService, IServiceBusService>();
            }
            else
            {
                messengerService = Substitute.For<IMessengerService, IQueueService>();
            }

            ILogger logger = Substitute.For<ILogger>();

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            string jobId = "3456";

            jobsApiClient
                .GetLatestJobsForSpecification("specificationId", Arg.Is<string[]>(_ => _.Single() == "PopulateScopedProviders"))
                .Returns(new ApiResponse<IDictionary<string, JobSummary>>(HttpStatusCode.OK, new Dictionary<string, JobSummary> { { string.Empty, new JobSummary { RunningStatus = RunningStatus.Completed, CompletionStatus = CompletionStatus.Failed, JobId = jobId } } }));

            messengerService
                .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobSummary>>(), TimeSpan.FromMilliseconds(600000))
                .Returns(new JobSummary
                {
                    CompletionStatus = CompletionStatus.Failed
                });

            //Act
            bool jobsComplete = await jobManagement.QueueJobAndWait(async () => await Task.Run(() => { return true; }), "PopulateScopedProviders", "specificationId", "correlationId", "topic");

            //Assert
            if (useServiceBus)
            {
                await ((IServiceBusService)messengerService)
                       .Received(1)
                       .CreateSubscription("topic", "correlationId", Arg.Is<TimeSpan>(_ => _.Days == 1));

                await messengerService
                    .Received(1)
                    .ReceiveMessage("topic/Subscriptions/correlationId", Arg.Any<Predicate<JobSummary>>(), TimeSpan.FromMilliseconds(600000));
            }

            jobsComplete
                .Should()
                .BeFalse();
        }

        [TestMethod]
        public async Task RetrieveJobAndCheckCanBeProcessed_ApiReturnsIncomplete_ReturnsCorrectly()
        {
            //Arrange
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            JobViewModel jvm = new JobViewModel
            {
                CompletionStatus = null
            };

            ApiResponse<JobViewModel> jobApiResponse = new ApiResponse<JobViewModel>(HttpStatusCode.OK, jvm);

            jobsApiClient
                .GetJobById(Arg.Any<string>())
                .Returns(jobApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            string jobId = "3456";

            //Act
            JobViewModel viewModel = await jobManagement.RetrieveJobAndCheckCanBeProcessed(jobId);

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
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            jobsApiClient
                .AddJobLog(Arg.Any<string>(), Arg.Any<JobLogUpdateModel>())
                .Returns(jobLogApiResponse);

            JobLogUpdateModel updateModel = new JobLogUpdateModel();

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            string jobId = "3456";

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
            yield return new object[]
            {
                new ApiResponse<JobLog>(HttpStatusCode.NotFound)
            };
            yield return new object[]
            {
                null
            };
        }

        [TestMethod]
        public async Task UpdateJobStatus_ApiResponseSuccess_Runs()
        {
            //Arrange
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            ApiResponse<JobLog> jobLogApiResponse = new ApiResponse<JobLog>(HttpStatusCode.OK, new JobLog());

            jobsApiClient
                .AddJobLog(Arg.Any<string>(), Arg.Any<JobLogUpdateModel>())
                .Returns(jobLogApiResponse);

            JobLogUpdateModel updateModel = new JobLogUpdateModel();

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            string jobId = "3456";

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

        [TestMethod]
        public async Task QueueJob_Called_ReturnsJob()
        {
            string specificationId = "1234";
            string jobId = "3456";

            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            Job jobApiResponse = new Job
            {
                Id = jobId
            };

            JobCreateModel jobCreateModel = new JobCreateModel
            {
                SpecificationId = specificationId
            };

            jobsApiClient
                .CreateJob(jobCreateModel)
                .Returns(jobApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            //Act
            await jobManagement.QueueJob(jobCreateModel);

            await jobsApiClient
                .Received(1)
                .CreateJob(jobCreateModel);
        }

        [TestMethod]
        public async Task QueueJobs_Called_ReturnsJobs()
        {
            string specificationId = "1234";
            string jobId = "3456";

            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            IEnumerable<Job> jobApiResponse = new List<Job>
            {
                new Job
                {
                    Id = jobId
                }
            };

            IEnumerable<JobCreateModel> jobCreateModel =
                new List<JobCreateModel>
                {
                    new JobCreateModel
                    {
                        SpecificationId = specificationId
                    }
                };

            jobsApiClient
                .CreateJobs(jobCreateModel)
                .Returns(jobApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            //Act
            await jobManagement.QueueJobs(jobCreateModel);

            await jobsApiClient
                .Received(1)
                .CreateJobs(jobCreateModel);
        }
        
        [TestMethod]
        public async Task TryQueueJobs_Called_ReturnsJobCreateResults()
        {
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            JobCreateModel createModelOne = new JobCreateModel();
            JobCreateModel createModelTwo = new JobCreateModel();
            
            JobCreateResult expectedCreateResultOne = new JobCreateResult();
            JobCreateResult expectedCreateResultTwo = new JobCreateResult();

            IEnumerable<JobCreateModel> createModels =
                new []
                {
                    createModelOne,
                    createModelTwo
                };

            jobsApiClient
                .TryCreateJobs(Arg.Is<IEnumerable<JobCreateModel>>(_ => _.SequenceEqual(createModels)))
                .Returns(new ApiResponse<IEnumerable<JobCreateResult>>(HttpStatusCode.OK, new[]
                {
                    expectedCreateResultOne,
                    expectedCreateResultTwo
                }));

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            //Act
            IEnumerable<JobCreateResult> actualResult = await jobManagement.TryQueueJobs(createModels);

            actualResult
                .Should()
                .BeEquivalentTo<JobCreateResult>(new []
                {
                    expectedCreateResultOne, 
                    expectedCreateResultTwo
                });
        }
        
        [TestMethod]
        public async Task TryQueueJob_Called_ReturnsJobCreateResult()
        {
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            JobCreateModel createModel = new JobCreateModel();
            
            JobCreateResult expectedCreateResult = new JobCreateResult();

            IEnumerable<JobCreateModel> createModels =
                new []
                {
                    createModel
                };

            jobsApiClient
                .TryCreateJobs(Arg.Is<IEnumerable<JobCreateModel>>(_ => _.SequenceEqual(createModels)))
                .Returns(new ApiResponse<IEnumerable<JobCreateResult>>(HttpStatusCode.OK, new[]
                {
                    expectedCreateResult
                }));

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            //Act
            JobCreateResult actualResult = await jobManagement.TryQueueJob(createModel);

            actualResult
                .Should()
                .BeSameAs(expectedCreateResult);
        }
        
        [TestMethod]
        public void TryQueueJob_ThrowsJobsNotCreatedExceptionIfApiResponseNull()
        {
            string specificationId = "1234";

            JobManagement jobManagement = new JobManagement(Substitute.For<IJobsApiClient>(), 
                Logger.None, 
                new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            }, 
                Substitute.For<IMessengerService>());

            Func<Task<JobCreateResult>> invocation = () => jobManagement.TryQueueJob(new JobCreateModel
            {
                SpecificationId = specificationId
            });

            invocation
                .Should()
                .Throw<JobsNotCreatedException>();
        }

        [TestMethod]
        public async Task GetLatestJobForSpecification_Called_ReturnsJobSummary()
        {
            string specificationId = "1234";
            string jobType = "3456";
            string jobId = "5678";

            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            string[] jobTypes = new string[]
            {
                jobType
            };

            IDictionary<string, JobSummary> jobSummary = new Dictionary<string, JobSummary> { { string.Empty, new JobSummary { JobId = jobId } } };
            ApiResponse<IDictionary<string, JobSummary>> jobSummaryApiResponse = new ApiResponse<IDictionary<string, JobSummary>>(HttpStatusCode.OK, jobSummary);

            jobsApiClient
                .GetLatestJobsForSpecification(specificationId, jobTypes)
                .Returns(jobSummaryApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            //Act
            IDictionary<string, JobSummary> result = await jobManagement.GetLatestJobsForSpecification(specificationId, jobTypes);

            Assert.AreEqual(result, jobSummary);

            await jobsApiClient
                .Received(1)
                .GetLatestJobsForSpecification(specificationId, jobTypes);
        }

        [TestMethod]
        public void GetLatestJobForSpecification_UnsuccessfulApiResponse_ThrowsJobsNotRetrievedException()
        {
            string specificationId = "1234";
            string jobType = "3456";

            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            string[] jobTypes = new string[]
            {
                jobType
            };

            string message = $"Error while retrieving latest jobs for Specifiation: {specificationId} and JobTypes: {string.Join(',', jobTypes)}";

            ApiResponse<IDictionary<string, JobSummary>> jobSummaryApiResponse = new ApiResponse<IDictionary<string, JobSummary>>(HttpStatusCode.NotFound);

            jobsApiClient
                .GetLatestJobsForSpecification(specificationId, jobTypes)
                .Returns(jobSummaryApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            //Act
            Func<Task> invocation = async () => await jobManagement.GetLatestJobsForSpecification(specificationId, jobTypes);

            //Arrange
            invocation.Should()
                .Throw<JobsNotRetrievedException>()
                .WithMessage(message);
        }

        [TestMethod]
        public async Task AddJobLog_Called_ReturnsJobLog()
        {
            string jobId = "5678";

            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            JobLog jobLog = new JobLog
            {
                JobId = jobId
            };
            ApiResponse<JobLog> jobLogApiResponse = new ApiResponse<JobLog>(HttpStatusCode.OK, jobLog);

            JobLogUpdateModel jobLogUpdateModel = new JobLogUpdateModel();

            jobsApiClient
                .AddJobLog(jobId, jobLogUpdateModel)
                .Returns(jobLogApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            //Act
            JobLog result = await jobManagement.AddJobLog(jobId, jobLogUpdateModel);

            Assert.AreEqual(result, jobLog);

            await jobsApiClient
                .Received(1)
                .AddJobLog(jobId, jobLogUpdateModel);
        }

        [TestMethod]
        public async Task GetNonCompletedJobsWithinTimeFrame_Called_ReturnsJobSummaries()
        {
            string jobId = "5678";

            DateTimeOffset from = DateTimeOffset.UtcNow.AddDays(-2);
            DateTimeOffset to = DateTimeOffset.UtcNow.AddDays(-1);

            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();


            JobSummary jobSummary = new JobSummary
            {
                JobId = jobId
            };
            IEnumerable<JobSummary> jobSummaries = new List<JobSummary>
            {
                jobSummary
            };
            ApiResponse<IEnumerable<JobSummary>> jobSummariesApiResponse
                = new ApiResponse<IEnumerable<JobSummary>>(HttpStatusCode.OK, jobSummaries);

            jobsApiClient
                .GetNonCompletedJobsWithinTimeFrame(from, to)
                .Returns(jobSummariesApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            //Act
            IEnumerable<JobSummary> result = await jobManagement.GetNonCompletedJobsWithinTimeFrame(from, to);

            Assert.AreEqual(result, jobSummaries);

            await jobsApiClient
                .Received(1)
                .GetNonCompletedJobsWithinTimeFrame(from, to);
        }

        [TestMethod]
        public async Task GetJobDetails_ReturnsJobViewModel()
        {
            IJobsApiClient jobsApiClient = Substitute.For<IJobsApiClient>();
            JobManagementResiliencePolicies policies = new JobManagementResiliencePolicies
            {
                JobsApiClient = Policy.NoOpAsync()
            };
            IMessengerService messengerService = Substitute.For<IMessengerService>();
            ILogger logger = Substitute.For<ILogger>();

            JobViewModel jvm = new JobViewModel
            {
                CompletionStatus = null
            };

            ApiResponse<JobViewModel> jobApiResponse = new ApiResponse<JobViewModel>(HttpStatusCode.OK, jvm);

            jobsApiClient
                .GetJobById(Arg.Any<string>())
                .Returns(jobApiResponse);

            JobManagement jobManagement = new JobManagement(jobsApiClient, logger, policies, messengerService);

            string jobId = "3456";

            //Act
            JobViewModel viewModel = await jobManagement.GetJobById(jobId);

            //Assert    
            viewModel
                .Should()
                .Be(jvm);
        }
    }
}