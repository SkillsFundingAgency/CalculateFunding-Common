using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Jobs.UnitTests
{
    [TestClass]
    public class JobsApiClientTests : ApiClientTestBase
    {
        private JobsApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new JobsApiClient(ClientFactory,
                Logger.None);
        }

        [TestMethod]
        public async Task AddJobLog()
        {
            string id = NewRandomString();
            JobLogUpdateModel jobLogUpdateModel = new JobLogUpdateModel();

            await AssertPostRequest($"jobs/{id}/logs",
                jobLogUpdateModel,
                new JobLog(),
                () => _client.AddJobLog(id, jobLogUpdateModel));
        }

        [TestMethod]
        public async Task GetJobById()
        {
            string id = NewRandomString();

            await AssertGetRequest($"jobs/{id}",
                id,
                new JobViewModel(),
                _client.GetJobById);
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow(new string[0], null)]
        [DataRow(new [] {"one", "two"}, "&jobDefinitionIds=one&jobDefinitionIds=two")]
        public async Task GetLatestJobForSpecification(string[] jobTypes,
            string expectedJobsParameters)
        {
            string specificationId = NewRandomString();
            IDictionary<string, JobSummary> jobsummaries = new Dictionary<string, JobSummary> { { "", NewJobSummary() } };
            await AssertGetRequest($"jobs/latest?specificationId={specificationId}{expectedJobsParameters}",
                jobsummaries,
                () => _client.GetLatestJobsForSpecification(specificationId, jobTypes));
        }

        [TestMethod]
        public async Task CreateJob()
        {
            JobCreateModel jobCreateModel = NewCreateModel();
            IEnumerable<Job> jobs = NewEnumerable(NewJob());
            
            GivenTheResponse("jobs", jobs, HttpMethod.Post);

            Job apiResponse = await _client.CreateJob(jobCreateModel);
            
            apiResponse
                .Should()
                .BeEquivalentTo(jobs.Single());
            
            AndTheRequestContentsShouldHaveBeen(NewEnumerable(jobCreateModel).AsJson());
        }
        
        [TestMethod]
        public async Task CreateJobs()
        {
            IEnumerable<JobCreateModel> jobCreateModels = NewEnumerable(NewCreateModel(), NewCreateModel());
            IEnumerable<Job> jobs = NewEnumerable(NewJob(), NewJob());
            
            GivenTheResponse("jobs", jobs, HttpMethod.Post);

            IEnumerable<Job> apiResponse = await _client.CreateJobs(jobCreateModels);
            
            apiResponse
                .Should()
                .BeEquivalentTo(jobs, 
                    cfg => cfg.WithStrictOrdering());
            
            AndTheRequestContentsShouldHaveBeen(jobCreateModels.AsJson());
        }

        [TestMethod]
        public async Task GetJobDefinitions()
        {
            await AssertGetRequest("jobs/jobdefinitions",
                Enumerable.Empty<JobDefinition>(),
                _client.GetJobDefinitions);
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow(new string[0], null)]
        [DataRow(new[] { "one", "two" }, "&jobDefinitionIds=one&jobDefinitionIds=two")]
        public async Task GetLatestJobByJobDefinitionIds(string[] jobTypes,
            string expectedJobsParameters)
        {
            IDictionary<string, JobSummary> jobsummaries = new Dictionary<string, JobSummary> { { "", NewJobSummary() } };
            await AssertGetRequest($"jobs/latest-by-job-definition-ids{expectedJobsParameters}",
                jobsummaries,
                () => _client.GetLatestJobsByJobDefinitionIds(jobTypes));
        }

        [TestMethod]
        public async Task GetJobDefinition()
        {
            string id = NewRandomString();
            
            await AssertGetRequest($"jobs/jobdefinitions/{id}",
                id,
                new JobDefinition(),
                _client.GetJobDefinition);
        }

        [TestMethod]
        public async Task GetNonCompletedJobsWithinTimeFrame()
        {
            DateTimeOffset from = DateTimeOffset.UtcNow;
            DateTimeOffset to = from.AddHours(new Random().Next(0, 24));

            await AssertGetRequest(
                $"jobs/noncompleted/dateTimeFrom/{from:yyyy-MM-ddTHH:mm:ss.fffZ}/dateTimeTo/{to:yyyy-MM-ddTHH:mm:ss.fffZ}",
                Enumerable.Empty<JobSummary>(),
                () => _client.GetNonCompletedJobsWithinTimeFrame(from, to));
        }
        
        [TestMethod]
        public async Task TryCreateJobs()
        {
            IEnumerable<JobCreateModel> jobCreateModels = NewEnumerable(NewCreateModel(), NewCreateModel());
            IEnumerable<JobCreateResult> jobs = NewEnumerable(NewCreateResult(), NewCreateResult());
            
            GivenTheResponse("jobs/try-create-jobs", jobs, HttpMethod.Post);

            ApiResponse<IEnumerable<JobCreateResult>> apiResponse = await _client.TryCreateJobs(jobCreateModels);
            
            apiResponse
                    ?.Content
                .Should()
                .BeEquivalentTo(jobs, 
                    cfg => cfg.WithStrictOrdering());
            
            AndTheRequestContentsShouldHaveBeen(jobCreateModels.AsJson());    
        }
        
        [TestMethod]
        public async Task TryCreateJob()
        {
            JobCreateModel jobCreateRequest = NewCreateModel();
            JobCreateResult result = NewCreateResult();
            
            GivenTheResponse("jobs/try-create-job", result, HttpMethod.Post);

            ApiResponse<JobCreateResult> apiResponse = await _client.TryCreateJob(jobCreateRequest);
            
            apiResponse?
                .Content
                .Should()
                .BeEquivalentTo(result);
            
            AndTheRequestContentsShouldHaveBeen(jobCreateRequest.AsJson());    
        }

        [TestMethod]
        public async Task GetLatestSuccessfulJobForSpecification()
        {
            string specificationId = NewRandomString();
            string jobDefinitonId = NewRandomString();
            JobSummary jobsummary = NewJobSummary();
            await AssertGetRequest($"jobs/latest-success?specificationId={specificationId}&jobDefinitionId={jobDefinitonId}",
                jobsummary,
                () => _client.GetLatestSuccessfulJobForSpecification(specificationId, jobDefinitonId));
        }

        [TestMethod]
        public async Task GetLatestJobByTriggerEntityId()
        {
            string specificationId = NewRandomString();
            string entityId = NewRandomString();
            JobSummary jobsummary = NewJobSummary();
            await AssertGetRequest($"jobs/latest-by-entity-id?specificationId={specificationId}&entityId={entityId}",
                jobsummary,
                () => _client.GetLatestJobByTriggerEntityId(specificationId, entityId));
        }

        private JobCreateModel NewCreateModel() => new JobCreateModel();
        
        private Job NewJob() => new Job();

        private JobSummary NewJobSummary() => new JobSummary();

        private JobViewModel NewJobViewModel() => new JobViewModel();

        private JobCreateResult NewCreateResult() => new JobCreateResult();
    }
}