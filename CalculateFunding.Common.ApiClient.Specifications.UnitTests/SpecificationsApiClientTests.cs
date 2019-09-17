using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;

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


        [DynamicData(nameof(MissingIdExamples), DynamicDataSourceType.Method)]
        [TestMethod]
        public async Task GetSpecificationSummaryByIdAsyncThrowsExceptionIfSuppliedSpecificationIdMissing(
            string specificationId)

        {
            Func<Task> invocation = () => WhenTheSpecificationSummaryIsQueriedById(specificationId);

            await invocation.Should()
                .ThrowExactlyAsync<ArgumentNullException>(specificationId);
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

        private async Task<ApiResponse<IEnumerable<SpecificationSummary>>> WhenTheSpecificationSummaryIsQueriedForFundingByPeriod(
            string fundingPeriod)
        {
            return await _client.GetSpecificationsSelectedForFundingByPeriod(fundingPeriod);
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
    }
}