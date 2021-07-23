using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Calcs.Models;
using CalculateFunding.Common.ApiClient.Calcs.Models.Code;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Calcs.Tests
{
    [TestClass]
    public class CalculationsApiClientTests : ApiClientTestBase
    {
        private CalculationsApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new CalculationsApiClient(ClientFactory,
                Logger.None);
        }

        [TestMethod]
        [DataRow("spec1", CalculationType.Additional, null, null, 10,
            "specifications/spec1/calculations/calculationType/Additional?page=10")]
        [DataRow("spec2", CalculationType.Template, null, "find me", null,
            "specifications/spec2/calculations/calculationType/Template?searchTerm=find+me")]
        [DataRow("spec3", CalculationType.Additional, null, "and find me", 20,
            "specifications/spec3/calculations/calculationType/Additional?searchTerm=and+find+me&page=20")]
        [DataRow("spec3", CalculationType.Additional, PublishStatus.Updated, "and find me", 20,
            "specifications/spec3/calculations/calculationType/Additional?searchTerm=and+find+me&page=20&status=Updated")]
        [DataRow("spec4", CalculationType.Template, PublishStatus.Approved, null, null,
            "specifications/spec4/calculations/calculationType/Template?status=Approved")]
        public async Task SearchCalculationsForSpecification(string specificationId,
            CalculationType calculationType,
            PublishStatus? status,
            string searchTerm,
            int? page,
            string expectedUri)
        {
            await AssertGetRequest(expectedUri,
                new SearchResults<CalculationSearchResult>(),
                () => _client.SearchCalculationsForSpecification(specificationId,
                    calculationType,
                    status,
                    searchTerm,
                    page));
        }

        [TestMethod]
        public async Task GetCalculationSummariesForSpecification()
        {
            string id = NewRandomString();

            await AssertGetRequest($"calculation-summaries-for-specification?specificationId={id}",
                id,
                Enumerable.Empty<CalculationSummary>(),
                 _client.GetCalculationSummariesForSpecification);
        }

        [TestMethod]
        public async Task GetBuildProjectBySpecificationId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-buildproject-by-specification-id?specificationId={id}",
                id,
                new BuildProject(),
                _client.GetBuildProjectBySpecificationId);
        }

        [TestMethod]
        public async Task GetAssemblyBySpecificationId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"{id}/assembly",
                id,
                new byte[0],
                _client.GetAssemblyBySpecificationId);
        }

        [TestMethod]
        public async Task UpdateBuildProjectRelationships()
        {
            string id = NewRandomString();
            string expectedUri = $"update-buildproject-relationships?specificationId={id}";

            BuildProject expectedResponse = new BuildProject();
            DatasetRelationshipSummary expectedRequest = new DatasetRelationshipSummary();

            GivenTheResponse(expectedUri, expectedResponse, HttpMethod.Post);

            ApiResponse<BuildProject> apiResponse = await _client.UpdateBuildProjectRelationships(id, expectedRequest);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse
                .Content
                .Should()
                .BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task GetCalculationsForSpecification()
        {
            string id = NewRandomString();

            await AssertGetRequest($"current-calculations-for-specification?specificationId={id}",
                id,
                Enumerable.Empty<Calculation>(),
                _client.GetCalculationsForSpecification);
        }

        [TestMethod]
        public async Task CompileAndSaveAssembly()
        {
            string id = NewRandomString();

            GivenTheStatusCode($"{id}/compileAndSaveAssembly", HttpStatusCode.OK, HttpMethod.Get);

            ApiResponse<HttpStatusCode> apiResponse = await _client.CompileAndSaveAssembly(id);

            apiResponse?.Content
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetCalculationById()
        {
            string id = NewRandomString();

            await AssertGetRequest($"/calculation-by-id?calculationId={id}",
                id,
                new Calculation(),
                _client.GetCalculationById);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.OK, true)]
        [DataRow(HttpStatusCode.InternalServerError, false)]
        [DataRow(HttpStatusCode.NotFound, false)]
        public async Task IsCalculationNameValid(HttpStatusCode statusCode,
            bool expectedFlag)
        {
            string specificationId = NewRandomString();
            string existingCalculationId = NewRandomString();
            string calculationName = NewRandomString();

            GivenTheStatusCode($"validate-calc-name/{specificationId}/{calculationName}/{existingCalculationId}",
                statusCode,
                HttpMethod.Get);

            ApiResponse<bool> apiResponse = await _client.IsCalculationNameValid(specificationId, calculationName, existingCalculationId);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse
                .Content
                .Should()
                .Be(expectedFlag);
        }

        [TestMethod]
        public async Task CreateCalculation()
        {
            string id = NewRandomString();

            CalculationCreateModel calculationCreateModel = new CalculationCreateModel();

            await AssertPostRequest($"specifications/{id}/calculations",
                calculationCreateModel,
                new Calculation(),
                () => _client.CreateCalculation(id, calculationCreateModel));
        }

        [TestMethod]
        public async Task EditCalculation()
        {
            string specificationId = NewRandomString();
            string calculationId = NewRandomString();
            CalculationEditModel calculationEditModel = new CalculationEditModel();

            await AssertPutRequest($"specifications/{specificationId}/calculations/{calculationId}",
                calculationEditModel,
                new Calculation(),
                () => _client.EditCalculation(specificationId, calculationId, calculationEditModel));
        }

        [TestMethod]
        public async Task EditCalculationWithSkipInstruct()
        {
            string specificationId = NewRandomString();
            string calculationId = NewRandomString();
            CalculationEditModel calculationEditModel = new CalculationEditModel();

            await AssertPutRequest($"specifications/{specificationId}/calculations/{calculationId}/true",
                calculationEditModel,
                new Calculation(),
                () => _client.EditCalculationWithSkipInstruct(specificationId, calculationId, calculationEditModel));
        }

        [TestMethod]
        public async Task PreviewCompile()
        {
            await AssertPostRequest("compile-preview",
                new PreviewRequest(),
                new PreviewResponse(),
                _ => _client.PreviewCompile(_));
        }

        [TestMethod]
        public async Task GetAllVersionsByCalculationId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"calculation-version-history?calculationId={id}",
                id,
                Enumerable.Empty<CalculationVersion>(),
                _client.GetAllVersionsByCalculationId);
        }

        [TestMethod]
        public async Task GetMultipleVersionsByCalculationId()
        {
            int[] versionIds = new[]
            {
                NewRandomInt(),
                NewRandomInt(),
                NewRandomInt(),
            };
            string calculationId = NewRandomString();

            CalculationVersionsCompareModel expectedRequest = new CalculationVersionsCompareModel
            {
                Versions = versionIds,
                CalculationId = calculationId
            };

            IEnumerable<CalculationVersion> expectedResponse = Enumerable.Empty<CalculationVersion>();

            GivenTheResponse("calculation-versions", expectedResponse, HttpMethod.Post);

            ApiResponse<IEnumerable<CalculationVersion>> apiResponse = await _client.GetMultipleVersionsByCalculationId(versionIds, calculationId);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse
                .Content
                .Should()
                .BeEquivalentTo(expectedResponse);

            AndTheRequestContentsShouldHaveBeen(expectedRequest.AsJson());
        }

        [TestMethod]
        public async Task GetCodeContextForSpecification()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-calculation-code-context?specificationId={id}",
                id,
                Enumerable.Empty<TypeInformation>(),
                _client.GetCodeContextForSpecification);
        }

        [TestMethod]
        public async Task UpdatePublishStatus()
        {
            string id = NewRandomString();
            PublishStatusEditModel request = new PublishStatusEditModel();

            await AssertPutRequest($"calculation-edit-status?calculationId={id}",
                request,
                new PublishStatusResult(),
                () => _client.UpdatePublishStatus(id, request));
        }

        [TestMethod]
        public async Task GetCalculationStatusCounts()
        {
            await AssertPostRequest("status-counts",
                new SpecificationIdsRequestModel(),
                Enumerable.Empty<CalculationStatusCounts>(),
                _client.GetCalculationStatusCounts);
        }

        [TestMethod]
        public async Task FindCalculations()
        {
            SearchFilterRequest input = new SearchFilterRequest();
            SearchQueryRequest expectedRequest = SearchQueryRequest.FromSearchFilterRequest(input);
            SearchResults<CalculationSearchResult> expectedResults = new SearchResults<CalculationSearchResult>();

            GivenTheResponse("calculations-search", expectedResults, HttpMethod.Post);

            ApiResponse<SearchResults<CalculationSearchResult>> apiResponse = await _client.FindCalculations(input);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse
                .Content
                .Should()
                .BeEquivalentTo(expectedResults);

            AndTheRequestContentsShouldHaveBeen(expectedRequest.AsJson());
        }

        [TestMethod]
        public async Task GetCalculationMetadataForSpecification()
        {
            string id = NewRandomString();

            await AssertGetRequest($"specifications/{id}/calculations/metadata",
                id,
                Enumerable.Empty<CalculationMetadata>(),
                _client.GetCalculationMetadataForSpecification);
        }

        [TestMethod]
        public async Task GetTemplateMapping()
        {
            string specificationId = NewRandomString();
            string fundingStreamId = NewRandomString();

            await AssertGetRequest($"specifications/{specificationId}/templatemapping/{fundingStreamId}",
                new TemplateMapping(),
                () => _client.GetTemplateMapping(specificationId, fundingStreamId));
        }

        [TestMethod]
        public async Task CheckHasAllApprovedTemplateCalculationsForSpecificationId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"specifications/{id}/templateCalculations/allApproved",
                id,
                new BooleanResponseModel(),
                _client.CheckHasAllApprovedTemplateCalculationsForSpecificationId);
        }

        [TestMethod]
        public async Task AssociateTemplateIdWithSpecification()
        {
            string specificationId = NewRandomString();
            string templateVersion = NewRandomString();
            string fundingStreamId = NewRandomString();

            TemplateMapping expectedResponse = new TemplateMapping();

            GivenTheResponse($"specifications/{specificationId}/templates/{fundingStreamId}",
                expectedResponse,
                HttpMethod.Put);

            ApiResponse<TemplateMapping> apiResponse = await _client.ProcessTemplateMappings(specificationId,
                templateVersion,
                fundingStreamId);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse
                .Content
                .Should()
                .BeEquivalentTo(expectedResponse);

            AndTheRequestContentsShouldHaveBeen(templateVersion.AsJson());
        }

        [TestMethod]
        public async Task GetRootFundingLinesForCalculation()
        {
            string id = NewRandomString();

            await AssertGetRequest($"{id}/root-funding-lines",
                new[]
                {
                    new CalculationFundingLine
                    {
                        Name = NewRandomString(),
                        TemplateId = (uint)NewRandomInt()
                    }
                }.AsEnumerable(),
                () => _client.GetRootFundingLinesForCalculation(id));
        }

        [TestMethod]
        public async Task QueueCodeContextUpdate()
        {
            string specificationId = NewRandomString();

            Job expectedJob = new Job
            {
                Id = NewRandomString()
            };

            GivenTheResponse($"specifications/{specificationId}/code-context/update", expectedJob, HttpMethod.Post);

            ApiResponse<Job> apiResponse = await _client.QueueCodeContextUpdate(specificationId);

            apiResponse?
                .Content
                .Should()
                .BeEquivalentTo(expectedJob);
        }

        [TestMethod]
        public async Task QueueApproveAllSpecificationCalculations()
        {
            string specificationId = NewRandomString();

            Job expectedJob = new Job
            {
                Id = NewRandomString()
            };

            GivenTheResponse($"specifications/{specificationId}/approve-all-calculations", expectedJob, HttpMethod.Post);

            ApiResponse<Job> apiResponse = await _client.QueueApproveAllSpecificationCalculations(specificationId);

            apiResponse?
                .Content
                .Should()
                .BeEquivalentTo(expectedJob);
        }

        [TestMethod]
        public async Task GetObsoleteItemsForSpecification()
        {
            string specificationId = NewRandomString();
            IEnumerable<ObsoleteItem> expectedObsoleteItems = new List<ObsoleteItem>();

            GivenTheResponse($"obsoleteitems/specifications/{specificationId}", expectedObsoleteItems, HttpMethod.Get);

            ApiResponse<IEnumerable<ObsoleteItem>> apiResponse = await _client.GetObsoleteItemsForSpecification(specificationId);

            apiResponse?
                .Content
                .Should()
                .BeEquivalentTo(expectedObsoleteItems);
        }

        [TestMethod]
        public async Task CreateObsoleteItem()
        {
            ObsoleteItem expectedObsoleteItem = new ObsoleteItem();

            GivenTheResponse($"obsoleteitems", expectedObsoleteItem, HttpMethod.Post);

            ApiResponse<ObsoleteItem> apiResponse = await _client.CreateObsoleteItem(new ObsoleteItem());

            apiResponse?
                .Content
                .Should()
                .BeEquivalentTo(expectedObsoleteItem);
        }

        [TestMethod]
        public async Task GetObsoleteItemsForCalculation()
        {
            string calculationId = NewRandomString();
            IEnumerable<ObsoleteItem> expectedObsoleteItems = new List<ObsoleteItem>();

            GivenTheResponse($"obsoleteitems/calculations/{calculationId}", expectedObsoleteItems, HttpMethod.Get);

            ApiResponse<IEnumerable<ObsoleteItem>> apiResponse = await _client.GetObsoleteItemsForCalculation(calculationId);

            apiResponse?
                .Content
                .Should()
                .BeEquivalentTo(expectedObsoleteItems);


        }

        [TestMethod]
        public async Task RemoveObsoleteItem()
        {
            string obsoleteItemId = NewRandomString();
            string calculationId = NewRandomString();

            GivenTheStatusCode($"obsoleteitems/{obsoleteItemId}/{calculationId}", HttpStatusCode.NoContent, HttpMethod.Delete);

            HttpStatusCode apiResponse = await _client.RemoveObsoleteItem(obsoleteItemId, calculationId);

            apiResponse
                .Should()
                .Be(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task AddCalculationToObsoleteItem()
        {
            string obsoleteItemId = NewRandomString();
            string calculationId = NewRandomString();

            GivenTheStatusCode($"obsoleteitems/{obsoleteItemId}/{calculationId}", HttpStatusCode.OK, HttpMethod.Patch);

            HttpStatusCode apiResponse = await _client.AddCalculationToObsoleteItem(obsoleteItemId, calculationId);

            apiResponse
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task ReMapSpecification()
        {
            string specificationId = NewRandomString();
            string datasetRelationshipId = NewRandomString();

            Job expectedJob = new Job();

            GivenTheResponse($"calcs/{specificationId}/{datasetRelationshipId}/remap", expectedJob, HttpMethod.Post);

            ApiResponse<Job> apiResponse = await _client.ReMapSpecificationReference(specificationId, datasetRelationshipId);

            apiResponse
                .Content
                .Should()
                .BeEquivalentTo(expectedJob);
        }
    }
}