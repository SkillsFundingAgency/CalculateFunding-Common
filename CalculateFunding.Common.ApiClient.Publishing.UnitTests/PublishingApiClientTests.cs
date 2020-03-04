using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Publishing.Models;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Publishing.UnitTests
{
    [TestClass]
    public class PublishingApiClientTests : ApiClientTestBase
    {
        private PublishingApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new PublishingApiClient(ClientFactory,
                Substitute.For<ILogger>());
        }

        [TestMethod]
        public async Task GetLatestProfileTotalsGetsProfileTotals()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string providerId = NewRandomString();
            
            string expectedUri = $"publishedproviders/{fundingStreamId}/{fundingPeriodId}/{providerId}/profileTotals";
            
            IEnumerable<ProfileTotal> expectedTotals = new[]
            {
                new ProfileTotal(),
            };
            
            GivenTheResponse(expectedUri, expectedTotals, HttpMethod.Get);

            ApiResponse<IEnumerable<ProfileTotal>> response = await _client.GetLatestProfileTotals(fundingStreamId,
                fundingPeriodId,
                providerId);

            response?.Content
                .Should()
                .BeEquivalentTo(expectedTotals);
            
            AndTheUrisShouldHaveBeenRequested(expectedUri);
        }

        [TestMethod]
        public async Task GetAllReleasedProfileTotalsGetsAllReleasedProfileTotals()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string providerId = NewRandomString();

            string expectedUri = $"publishedproviders/{fundingStreamId}/{fundingPeriodId}/{providerId}/allProfileTotals";

            PublishedProviderVersion publishedProviderVersion = new PublishedProviderVersion { Version = 1, Date = DateTimeOffset.MinValue };

            IEnumerable<ProfileTotal> expectedTotals = new[]
            {
                new ProfileTotal(),
            };

            IDictionary<int, ProfilingVersion> expectedProfiling = new Dictionary<int, ProfilingVersion>(new[] { new KeyValuePair<int, 
                ProfilingVersion>(publishedProviderVersion.Version, 
                new ProfilingVersion { Date = publishedProviderVersion.Date, 
                    Version = publishedProviderVersion.Version, 
                    ProfileTotals = expectedTotals }) });

            GivenTheResponse(expectedUri, expectedProfiling, HttpMethod.Get);

            ApiResponse<IDictionary<int, ProfilingVersion>> response = await _client.GetAllReleasedProfileTotals(fundingStreamId,
                fundingPeriodId,
                providerId);

            response?.Content
                .Should()
                .BeEquivalentTo(expectedProfiling);

            AndTheUrisShouldHaveBeenRequested(expectedUri);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.OK)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.InsufficientStorage)]
        public async Task RefreshFundingForSpecificationPostsToPublishingService(HttpStatusCode expectedStatusCode)
        {
            string specificationId = NewRandomString();
            string expectedUri = $"specifications/{specificationId}/refresh";

            GivenTheStatusCode(expectedUri, expectedStatusCode, HttpMethod.Post);

            ValidatedApiResponse<JobCreationResponse> response = await _client.RefreshFundingForSpecification(specificationId);

            response
                .Should()
                .NotBeNull();

            response
                .StatusCode
                .Should()
                .Be(expectedStatusCode);

            AndTheUrisShouldHaveBeenRequested(expectedUri);
        }

        [TestMethod]
        [DataRow(HttpStatusCode.OK)]
        [DataRow(HttpStatusCode.BadGateway)]
        [DataRow(HttpStatusCode.InternalServerError)]
        public async Task ApproveFundingForSpecificationPostsToPublishingService(HttpStatusCode expectedStatusCode)
        {
            string specificationId = NewRandomString();
            string expectedUri = $"specifications/{specificationId}/approve";

            GivenTheStatusCode(expectedUri, expectedStatusCode, HttpMethod.Post);

            ValidatedApiResponse<JobCreationResponse> response = await _client.ApproveFundingForSpecification(specificationId);

            response
                 .Should()
                 .NotBeNull();

            response
                .StatusCode
                .Should()
                .Be(expectedStatusCode);

            AndTheUrisShouldHaveBeenRequested(expectedUri);
        }
    }
}