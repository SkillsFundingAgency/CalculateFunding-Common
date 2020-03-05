using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Models;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Models.Versioning;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Specifications.UnitTests
{
    [TestClass]
    public class SpecificationsApiClientTests : ApiClientTestBase
    {
        private SpecificationsApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new SpecificationsApiClient(ClientFactory,
                Substitute.For<ILogger>());
        }

        [TestMethod]
        public async Task GetSpecificationsSelectedForFundingByPeriodAsyncMakesGetCallToRetrieveSpecificationSummariesByPeriodId()
        {
            string fundingPeriodId = NewRandomString();
            IEnumerable<SpecificationSummary> expectedSummarySpecification = new[]
            {
                NewSpecificationSummary(),
                NewSpecificationSummary(),
                NewSpecificationSummary()
            };

            string expectedGetByIdUri = GetSummariesByFundingPeriodIdUriFor(fundingPeriodId);

            GivenTheResponse($"specs/specifications-selected-for-funding-by-period?fundingPeriodId={fundingPeriodId}", expectedSummarySpecification, HttpMethod.Get);

            ApiResponse<IEnumerable<SpecificationSummary>> apiResponse =
                await WhenTheSpecificationSummaryIsQueriedForFundingByPeriod(fundingPeriodId);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse?.Content
                .Select(_ => _.Id)
                .Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(expectedSummarySpecification.Select(_ => _.Id));

            AndTheUrisShouldHaveBeenRequested(expectedGetByIdUri);
        }

        [DynamicData(nameof(MissingIdExamples), DynamicDataSourceType.Method)]
        [TestMethod]
        public async Task GetSpecificationsSelectedForFundingByPeriodAsyncThrowsExceptionIfSuppliedSpecificationIdMissing(
            string fundingPeriodId)

        {
            Func<Task> invocation = () => WhenTheSpecificationSummaryIsQueriedForFundingByPeriod(fundingPeriodId);

            await invocation.Should()
                .ThrowExactlyAsync<ArgumentNullException>(fundingPeriodId);
        }


        [TestMethod]
        public async Task GetSpecificationSummaryByIdAsyncMakesGetCallToRetrieveSpecificationSummaryBySuppliedId()
        {
            string specificationId = NewRandomString();
            SpecificationSummary expectedSummarySpecification = NewSpecificationSummary();
            string expectedGetByIdUri = GetSummaryByIdUriFor(specificationId);

            GivenTheResponse($"specs/specification-summary-by-id?specificationId={specificationId}", expectedSummarySpecification, HttpMethod.Get);

            ApiResponse<SpecificationSummary> apiResponse = await WhenTheSpecificationSummaryIsQueriedById(specificationId);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse?.Content
                .Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(expectedSummarySpecification);

            AndTheUrisShouldHaveBeenRequested(expectedGetByIdUri);
        }

        [TestMethod]
        public async Task GetDistinctFundingStreamsForSpecifciations()
        {           

            GivenTheResponse($"specs/fundingstream-id-for-specifications", _fundingStreamIds, HttpMethod.Get);

            ApiResponse<IEnumerable<string>> apiResponse = await WhenGetDistinctFundingStreamsForSpecification();

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse?.Content
                .Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(_fundingStreamIds);

           
        }


        [DynamicData(nameof(MissingIdExamples), DynamicDataSourceType.Method)]
        [TestMethod]
        public async Task GetSpecificationSummaryByIdAsyncThrowsExceptionIfSuppliedSpecificationIdMissing(
            string specificationId)

        {
            Func<Task> invocation = () => WhenTheSpecificationSummaryIsQueriedById(specificationId);

            await invocation.Should()
                .ThrowExactlyAsync<ArgumentNullException>(specificationId);
        }
        
        [DynamicData(nameof(MissingIdExamples), DynamicDataSourceType.Method)]
        [TestMethod]
        public async Task GetProfileVariationPointersAsyncThrowsExceptionIfSuppliedSpecificationIdMissing(
            string specificationId)

        {
            Func<Task> invocation = () => WhenTheProfileVariationPointersAreQueriedById(specificationId);

            await invocation.Should()
                .ThrowExactlyAsync<ArgumentNullException>(specificationId);
        }

        [TestMethod]
        public async Task GetProfileVariationPointers()
        {
            ProfileVariationPointer[] expectedPointers = {
                NewProfileVariationPointer(),
                NewProfileVariationPointer(),
                NewProfileVariationPointer(),
            };

            string specificationId = NewRandomString();
            
            GivenTheResponse($"specs/{specificationId}/profilevariationpointers", expectedPointers, HttpMethod.Get);
            
            ApiResponse<IEnumerable<ProfileVariationPointer>> apiResponse = await WhenTheProfileVariationPointersAreQueriedById(specificationId);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse?.Content
                .Should()
                .NotBeNull()
                .And
                .BeEquivalentTo(expectedPointers);
        }

        [TestMethod]
        public async Task GetSpecificationByName()
        {
            string name = NewRandomString();

            await AssertGetRequest($"specification-by-name?specificationName={name}",
                name,
                new SpecificationSummary(),
                _client.GetSpecificationByName);
        }

        [TestMethod]
        public async Task SetAssignedTemplateVersion()
        {
            string specificationId = NewRandomString();
            string templateVersion = NewRandomString();
            string fundingStreamId = NewRandomString();

            await AssertPutRequest($"{specificationId}/templates/{fundingStreamId}",
                HttpStatusCode.OK,
                () => _client.SetAssignedTemplateVersion(specificationId, templateVersion, fundingStreamId));
            
            AndTheRequestContentsShouldHaveBeen(templateVersion.AsJson());
        }

        [TestMethod]
        public async Task SelectSpecificationForFunding()
        {
            string id = NewRandomString();

            await AssertPostRequest($"select-for-funding?specificationId={id}",
                HttpStatusCode.OK,
                () => _client.SelectSpecificationForFunding(id));
        }

        [TestMethod]
        public async Task DeselectSpecificationForFunding()
        {
            string id = NewRandomString();

            await AssertPostRequest($"deselect-for-funding/{id}",
                HttpStatusCode.OK,
                () => _client.DeselectSpecificationForFunding(id));    
        }

        [TestMethod]
        public async Task GetApprovedSpecifications()
        {
            string fundingPeriodId = NewRandomString();
            string fundingStreamId = NewRandomString();

            await AssertGetRequest(
                $"specifications-by-fundingperiod-and-fundingstream?fundingPeriodId={fundingPeriodId}&fundingStreamId={fundingStreamId}",
                Enumerable.Empty<SpecificationSummary>(),
                () => _client.GetApprovedSpecifications(fundingPeriodId, fundingStreamId));
        }

        [TestMethod]
        public async Task GetSpecificationsSelectedForFunding()
        {
            await AssertGetRequest("specifications-selected-for-funding",
                Enumerable.Empty<SpecificationSummary>(),
                _client.GetSpecificationsSelectedForFunding);
        }

        [TestMethod]
        public async Task FindSpecifications_SuccessfulSearch()
        {
            ApiResponse<SearchResults<SpecificationSearchResultItem>> expectedResponse = new ApiResponse<SearchResults<SpecificationSearchResultItem>>(HttpStatusCode.OK, 
                new SearchResults<SpecificationSearchResultItem>()
                {
                    Results = new [] { new SpecificationSearchResultItem() },
                });
            
            GivenTheResponse("specifications-search", expectedResponse, HttpMethod.Post);

            SearchModel search = NewRandomSearch();
            
            PagedResult<SpecificationSearchResultItem> apiResponse = await _client.FindSpecifications(new SearchFilterRequest
            {
                SearchTerm = search.SearchTerm
            });

            apiResponse
                .Should()
                .NotBeNull();
        }
        
        [TestMethod]
        public async Task FindSpecifications_UnSuccessfulSearch()
        {
            GivenTheStatusCode("specifications-search", HttpStatusCode.InternalServerError, HttpMethod.Post);

            PagedResult<SpecificationSearchResultItem> apiResponse = await _client.FindSpecifications(new SearchFilterRequest());

            apiResponse
                .Should()
                .BeNull();
        }

        [TestMethod]
        public async Task SetPublishDates()
        {
            string id = NewRandomString();
            SpecificationPublishDateModel model = new SpecificationPublishDateModel();

            await AssertPutRequest($"{id}/publishdates",
                model,
                HttpStatusCode.OK,
                () => _client.SetPublishDates(id, model));
        }

        [TestMethod]
        public async Task GetPublishDates()
        {
            string id = NewRandomString();

            await AssertGetRequest($"{id}/publishdates",
                new SpecificationPublishDateModel(),
                () => _client.GetPublishDates(id));
        }

        [TestMethod]
        public async Task GetSpecificationSummaries()
        {
            await AssertGetRequest("specification-summaries",
                Enumerable.Empty<SpecificationSummary>(),
                _client.GetSpecificationSummaries);
        }

        [TestMethod]
        public async Task UpdateSpecification()
        {
            await AssertPostRequest("specifications",
                new CreateSpecificationModel(),
                new SpecificationSummary(),
                _client.CreateSpecification);
        }

        [TestMethod]
        public async Task FindSpecificationAndRelationships_SuccessfulSearch()
        {
            ApiResponse<SearchResults<SpecificationDatasourceRelationshipSearchResultItem>> expectedResponse = new ApiResponse<SearchResults<SpecificationDatasourceRelationshipSearchResultItem>>(HttpStatusCode.OK, 
                new SearchResults<SpecificationDatasourceRelationshipSearchResultItem>()
                {
                    Results = new [] { new SpecificationDatasourceRelationshipSearchResultItem() },
                });
            
            GivenTheResponse("specifications-dataset-relationships-search", expectedResponse, HttpMethod.Post);

            SearchModel search = NewRandomSearch();
            
            PagedResult<SpecificationDatasourceRelationshipSearchResultItem> apiResponse = await _client.FindSpecificationAndRelationships(new SearchFilterRequest
            {
                SearchTerm = search.SearchTerm
            });

            apiResponse
                .Should()
                .NotBeNull();   
        }
        
        
        [TestMethod]
        public async Task FindSpecificationAndRelationships_UnSuccessfulSearch()
        {
            GivenTheStatusCode("specifications-dataset-relationships-search", HttpStatusCode.InternalServerError, HttpMethod.Post);

            PagedResult<SpecificationDatasourceRelationshipSearchResultItem> apiResponse = await _client.FindSpecificationAndRelationships(new SearchFilterRequest());

            apiResponse
                .Should()
                .BeNull();   
        }

        [TestMethod]
        public async Task GetSpecifications()
        {
            string id = NewRandomString();

            await AssertGetRequest($"specifications-by-year?fundingPeriodId={id}",
                Enumerable.Empty<SpecificationSummary>(),
                () => _client.GetSpecifications(id));
        }

        [TestMethod]
        public async Task GetSpecificationSummaries_ByIds()
        {
            IEnumerable<string> ids = NewEnumerable(NewRandomString(), NewRandomString());
            
            await AssertPostRequest("specification-summaries-by-ids",
                ids,
                Enumerable.Empty<SpecificationSummary>(),
                () => _client.GetSpecificationSummaries(ids));
        }

        [TestMethod]
        public async Task GetFundingStreamIdsForSelectedFundingSpecification()
        {
            await AssertGetRequest("fundingstream-ids-for-funding-specifications",
                NewEnumerable(NewRandomString()),
                _client.GetFundingStreamIdsForSelectedFundingSpecification);
        }

        [TestMethod]
        public async Task GetFundingPeriodsByFundingStreamIds()
        {
            string id = NewRandomString();

            await AssertGetRequest($"fundingperiods-by-fundingstream-id/{id}",
                id,
                Enumerable.Empty<Reference>(),
                _client.GetFundingPeriodsByFundingStreamIds);
        }

        [TestMethod]
        public async Task UpdateSpecificationStatus()
        {
            string id = NewRandomString();
            PublishStatusRequestModel model = new PublishStatusRequestModel();

            await AssertPutRequest($"specification-edit-status?specificationId={id}",
                model,
                new PublishStatusResponseModel(),
                () => _client.UpdateSpecificationStatus(id, model));
        }

        [TestMethod]
        public async Task GetDistinctFundingStreamsForSpecifications()
        {
            await AssertGetRequest("fundingstream-id-for-specifications",
                NewEnumerable(NewRandomString()),
                _client.GetDistinctFundingStreamsForSpecifications);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task DeleteSpecificationById(bool expectedResponse)
        {
            string id = NewRandomString();
            
            GivenThePrimitiveResponse($"?specificationId={id}", expectedResponse, HttpMethod.Get);

            ApiResponse<bool> apiResponse = await _client.DeleteSpecificationById(id);

            apiResponse?.Content
                .Should()
                .Be(expectedResponse);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task PermanentDeleteSpecificationById(bool expectedResponse)
        {
            string id = NewRandomString();
            
            GivenThePrimitiveResponse($"?specificationId={id}/permanent", expectedResponse, HttpMethod.Get);

            ApiResponse<bool> apiResponse = await _client.PermanentDeleteSpecificationById(id);

            apiResponse?.Content
                .Should()
                .Be(expectedResponse); 
        }
        
        private static string GetSummaryByIdUriFor(string specificationId)
        {
            return $"specs/specification-summary-by-id?specificationId={specificationId}";
        }

        private static string GetSummariesByFundingPeriodIdUriFor(string fundingPeriodId)
        {
            return $"specs/specifications-selected-for-funding-by-period?fundingPeriodId={fundingPeriodId}";
        }

        private async Task<ApiResponse<SpecificationSummary>> WhenTheSpecificationSummaryIsQueriedById(string specificationId)
        {
            return await _client.GetSpecificationSummaryById(specificationId);
        }
        
        private async Task<ApiResponse<IEnumerable<ProfileVariationPointer>>> WhenTheProfileVariationPointersAreQueriedById(string specificationId)
        {
            return await _client.GetProfileVariationPointers(specificationId);
        }

        private async Task<ApiResponse<IEnumerable<SpecificationSummary>>> WhenTheSpecificationSummaryIsQueriedForFundingByPeriod(
            string fundingPeriod)
        {
            return await _client.GetSpecificationsSelectedForFundingByPeriod(fundingPeriod);
        }

        private async Task<ApiResponse<IEnumerable<string>>> WhenGetDistinctFundingStreamsForSpecification()
        {
            return await _client.GetDistinctFundingStreamsForSpecifications();
        }

        private SpecificationSummary NewSpecificationSummary()
        {
            return new SummarySpecificationBuilder()
                .Build();
        }
        
        public static IEnumerable<string[]> MissingIdExamples()
        {
            yield return new[] {""};
            yield return new[] {string.Empty};
            yield return new[] {(string) null};
        }

        private IEnumerable<string> _fundingStreamIds = new List<string>() { "PSG", "DSG", "PSG1" };

        private ProfileVariationPointer NewProfileVariationPointer() => new ProfileVariationPointer();
    }
}