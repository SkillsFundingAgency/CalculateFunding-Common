using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.External.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Testing;
using CalculateFunding.Common.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;

namespace CalculateFunding.Common.ApiClient.External.UnitTests
{
    [TestClass]
    public class ExternalApiClientTests : ApiClientTestBase
    {
        private ExternalApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new ExternalApiClient(ClientFactory,
                HttpClientKeys.External,
                Substitute.For<ILogger>(),
                BearerTokenProvider);
        }

        [TestMethod]
        public async Task GetProviderFundingVersionMakesGetCallWithTheSuppliedProviderFundingVersion()
        {
            string providerFundingVersion = NewRandomString();

            AtomFeed<object> expectedAtomFeed = NewAtomFeed();

            GivenTheResponse($"v3/funding/provider/{providerFundingVersion}", expectedAtomFeed, HttpMethod.Get);

            ApiResponse<AtomFeed<object>> actualFeed =
                await WhenTheProviderFundingVersionIsQueriedByIdWithTheSuppliedProviderFundingVersion(providerFundingVersion);

            ThenTheResponseContentIs(actualFeed, expectedAtomFeed);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("")]
        public void GetProviderFundingVersionThrowsExceptionIfNoSuppliedProviderFundingVersion(string providerFundingVersion)
        {
            Func<Task<ApiResponse<AtomFeed<object>>>> invocation =
                () => WhenTheProviderFundingVersionIsQueriedByIdWithTheSuppliedProviderFundingVersion(providerFundingVersion);

            invocation
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task GetProviderFundingsMakesGetCallWithTheSuppliedProviderFundingVersion()
        {
            string providerFundingVersion = NewRandomString();

            IEnumerable<dynamic> expectedFundings = (new[] { new { fundingId = NewRandomString() }, 
                new { fundingId = NewRandomString() } }).AsJson().AsPoco<IEnumerable<dynamic>>();

            GivenTheResponse($"v3/funding/provider/{providerFundingVersion}/fundings", expectedFundings, HttpMethod.Get);

            ApiResponse<IEnumerable<dynamic>> actualFundings =
                await WhenTheProviderFundingsIsQueriedByIdWithTheSuppliedProviderFundingVersion(providerFundingVersion);

            ThenTheResponseContentIs(actualFundings, expectedFundings);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("")]
        public void GetProviderFundingsThrowsExceptionIfNoSuppliedProviderFundingVersion(string providerFundingVersion)
        {
            Func<Task<ApiResponse<IEnumerable<dynamic>>>> invocation =
                () => WhenTheProviderFundingsIsQueriedByIdWithTheSuppliedProviderFundingVersion(providerFundingVersion);

            invocation
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task GetFundingByIdMakesGetCallWithSuppliedFundingId()
        {
            string fundingId = NewRandomString();
            string expectedFunding = NewRandomString();

            GivenTheResponse($"v3/funding/byId/{fundingId}", expectedFunding, HttpMethod.Get);

            ApiResponse<string> actualFunding = await WhenTheFundingIsQueriedByIdWithTheSuppliedId(fundingId);

            ThenTheResponseContentIs(actualFunding, expectedFunding);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("")]
        public void GetFundingByIdMakesThrowsExceptionIfNoSuppliedId(string fundingId)
        {
            Func<Task<ApiResponse<string>>> invocation = () => WhenTheFundingIsQueriedByIdWithTheSuppliedId(fundingId);

            invocation
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DynamicData(nameof(NotificationsExamples), DynamicDataSourceType.Method)]
        public async Task GetNotificationsMakesGetWithTheSuppliedPageRef(string[] fundingStreamIds,
            string[] fundingPeriodIds,
            GroupingReason[] groupingReasons,
            int? pageRef,
            int? pageSize,
            string expectedUri)
        {
            AtomFeed<object> expectedFeed = NewAtomFeed();

            GivenTheResponse(expectedUri, expectedFeed, HttpMethod.Get);

            ApiResponse<AtomFeed<object>> actualFeed = await WhenTheNotificationsAreQueriedWithTheSuppliedPageRef(fundingStreamIds,
                fundingPeriodIds,
                groupingReasons,
                pageSize,
                pageRef);

            ThenTheResponseContentIs(actualFeed, expectedFeed);
        }

        [TestMethod]
        public async Task GetFundingStreamsMakesGetCall()
        {
            string _id = NewRandomString();
            string _name = NewRandomString();

            IEnumerable<FundingStream> expectedFunding = new List<FundingStream>
            {
                NewFundingStream(_ => _.WithId(_id).WithName(_name))
            };

            GivenTheResponse("v3/funding-streams", expectedFunding, HttpMethod.Get);

            ApiResponse<IEnumerable<FundingStream>> actualFundingStreams = await WhenGetFundingStreams();

            ThenTheResponseContentIs(actualFundingStreams, expectedFunding);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("")]
        public void GetFundingPeriodsMakesThrowsExceptionIfNoSuppliedId(string fundingStreamId)
        {
            Func<Task<ApiResponse<IEnumerable<FundingPeriod>>>> invocation = () =>
                WhenTheFundingPeriodIsQueriedByFundingStreamIdWithTheSuppliedId(fundingStreamId);

            invocation
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task GetFundingPeriodsMakesGetCallWithSuppliedFundingStreamId()
        {
            string fundingStreamId = NewRandomString();
            string _id = NewRandomString();
            string _name = NewRandomString();

            IEnumerable<FundingPeriod> expectedFunding = new List<FundingPeriod>
            {
                NewFundingPeriod(_ => _.WithId(_id).WithName(_name))
            };

            GivenTheResponse($"v3/funding-streams/{fundingStreamId}/funding-periods", expectedFunding, HttpMethod.Get);

            ApiResponse<IEnumerable<FundingPeriod>> actualFundingPeriods =
                await WhenTheFundingPeriodIsQueriedByFundingStreamIdWithTheSuppliedId(fundingStreamId);

            ThenTheResponseContentIs(actualFundingPeriods, expectedFunding);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("")]
        public void WhenTheTemplateSourceFileRetrieveThrowsExceptionIfNotSuppliedFundingStreamId(string fundingStreamId)
        {
            string fundingPeriodId = NewRandomString();
            string majorVersion = NewRandomString();
            string minorVersion = NewRandomString();

            Func<Task<ApiResponse<string>>> invocation = () =>
                WhenTheTemplateSourceFileRetrieved(fundingStreamId, fundingPeriodId, majorVersion, minorVersion);

            invocation
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("")]
        public void WhenTheTemplateSourceFileRetrieveThrowsExceptionIfNotSuppliedFundingPeriodId(string fundingPeriodId)
        {
            string fundingStreamId = NewRandomString();
            string majorVersion = NewRandomString();
            string minorVersion = NewRandomString();

            Func<Task<ApiResponse<string>>> invocation = () =>
                WhenTheTemplateSourceFileRetrieved(fundingStreamId, fundingPeriodId, majorVersion, minorVersion);

            invocation
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("")]
        public void WhenTheTemplateSourceFileRetrieveThrowsExceptionIfNotSuppliedMajorVersion(string majorVersion)
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string minorVersion = NewRandomString();

            Func<Task<ApiResponse<string>>> invocation = () =>
                WhenTheTemplateSourceFileRetrieved(fundingStreamId, fundingPeriodId, majorVersion, minorVersion);

            invocation
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(" ")]
        [DataRow("")]
        public void WhenTheTemplateSourceFileRetrieveThrowsExceptionIfNotSuppliedMinorVersion(string minorVersion)
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string majorVersion = NewRandomString();

            Func<Task<ApiResponse<string>>> invocation = () =>
                WhenTheTemplateSourceFileRetrieved(fundingStreamId, fundingPeriodId, majorVersion, minorVersion);

            invocation
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task GetFundingTemplateSourceFileGetCallWithSuppliedInput()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string majorVersion = NewRandomString();
            string minorVersion = NewRandomString();

            string response = NewRandomString();
            
            GivenTheResponse($"v3/funding-streams/{fundingStreamId}/funding-periods/{fundingPeriodId}/templates/{majorVersion}.{minorVersion}", response, HttpMethod.Get);

            ApiResponse<string> actualResponse =
                await WhenTheTemplateSourceFileRetrieved(fundingStreamId, fundingPeriodId, majorVersion, minorVersion);

            ThenTheResponseContentIs(actualResponse, response);
        }

        [TestMethod]
        public async Task GetPublishedFundingTemplatesMakesGetCallWithSuppliedFundingStreamIdAndFundingPeriodId()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
           

            IEnumerable<PublishedFundingTemplate> expectedFundingTemplates = new List<PublishedFundingTemplate>
            {
                new PublishedFundingTemplate()
            };

            GivenTheResponse($"v3/funding-streams/{fundingStreamId}/funding-periods/{fundingPeriodId}/templates", expectedFundingTemplates, HttpMethod.Get);

            ApiResponse<IEnumerable<PublishedFundingTemplate>> actualFundingPeriods =
                await WhenThePublishedFundingTemplatesIsQueriedByFundingStreamIdAndFundingPeriodId(fundingStreamId,fundingPeriodId);

            ThenTheResponseContentIs(actualFundingPeriods, expectedFundingTemplates);
        }

        public static IEnumerable<object[]> NotificationsExamples()
        {
            yield return new object []
            {
                null,
                null,
                null,
                null,
                null,
                "v3/funding/notifications"
            };
            yield return new object []
            {
                new [] { "4", "55" },
                null,
                null,
                null,
                12,
                "v3/funding/notifications?fundingStreamIds=4&fundingStreamIds=55&pageSize=12"
            };
            yield return new object []
            {
                new [] { "4", "55" },
                null,
                new [] { GroupingReason.Information },
                99,
                23,
                "v3/funding/notifications/99?fundingStreamIds=4&fundingStreamIds=55&groupingReasons=Information&pageSize=23"
            };
            yield return new object []
            {
                new [] { "3" },
                new [] {"45", "99", "902384"},
                new [] { GroupingReason.Payment, GroupingReason.Information },
                null,
                25,
                "v3/funding/notifications?fundingStreamIds=3&fundingPeriodIds=45&fundingPeriodIds=99&fundingPeriodIds=902384&groupingReasons=Payment&groupingReasons=Information&pageSize=25"
            };
        }

        private void ThenTheResponseContentIs<TContent>(ApiResponse<TContent> response, TContent expectedContent)
        {
            response
                .Should()
                .NotBeNull();

            response
                .StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            response
                .Content
                .Should()
                .BeEquivalentTo(expectedContent);
        }

        private async Task<ApiResponse<AtomFeed<object>>> WhenTheNotificationsAreQueriedWithTheSuppliedPageRef(string[] fundingStreamIds = null,
            string[] fundingPeriodIds = null,
            GroupingReason[] groupingReasons = null,
            int? pageSize = null,
            int? pageRef = null)
        {
            return await _client.GetFundingNotifications(fundingStreamIds, fundingPeriodIds, groupingReasons, pageSize: pageSize, pageRef: pageRef);
        }

        private async Task<ApiResponse<string>> WhenTheFundingIsQueriedByIdWithTheSuppliedId(string fundingId)
        {
            return await _client.GetFundingById(fundingId);
        }

        private async Task<ApiResponse<IEnumerable<FundingStream>>> WhenGetFundingStreams()
        {
            return await _client.GetFundingStreams();
        }

        private async Task<ApiResponse<IEnumerable<FundingPeriod>>> WhenTheFundingPeriodIsQueriedByFundingStreamIdWithTheSuppliedId(
            string fundingStreamId)
        {
            return await _client.GetFundingPeriods(fundingStreamId);
        }

        private async Task<ApiResponse<IEnumerable<PublishedFundingTemplate>>> WhenThePublishedFundingTemplatesIsQueriedByFundingStreamIdAndFundingPeriodId(
           string fundingStreamId, string fundingPeriodId)
        {
            return await _client.GetPublishedFundingTemplates(fundingStreamId, fundingPeriodId);
        }

        private async Task<ApiResponse<string>> WhenTheTemplateSourceFileRetrieved(
            string fundingStreamId, string fundingPeriodId, string majorVersion, string minorVersion)
        {
            return await _client.GetFundingTemplateSourceFile(fundingStreamId, fundingPeriodId, majorVersion, minorVersion);
        }

        private async Task<ApiResponse<AtomFeed<object>>> WhenTheProviderFundingVersionIsQueriedByIdWithTheSuppliedProviderFundingVersion(string providerFundingVersion)
        {
            return await _client.GetProviderFundingVersion(providerFundingVersion);
        }

        private AtomFeed<object> NewAtomFeed(Action<AtomFeedBuilder<object>> setUp = null)
        {
            AtomFeedBuilder<object> atomFeedBuilder = new AtomFeedBuilder<object>();

            setUp?.Invoke(atomFeedBuilder);

            return atomFeedBuilder.Build();
        }

        private FundingStream NewFundingStream(Action<FundingStreamBuilder> setUp = null)
        {
            FundingStreamBuilder fundingStreamBuilder = new FundingStreamBuilder();

            setUp?.Invoke(fundingStreamBuilder);

            return fundingStreamBuilder.Build();
        }

        private FundingPeriod NewFundingPeriod(Action<FundingPeriodBuilder> setUp = null)
        {
            FundingPeriodBuilder fundingPeriodBuilder = new FundingPeriodBuilder();

            setUp?.Invoke(fundingPeriodBuilder);

            return fundingPeriodBuilder.Build();
        }

        private async Task<ApiResponse<IEnumerable<dynamic>>> WhenTheProviderFundingsIsQueriedByIdWithTheSuppliedProviderFundingVersion(string providerFundingVersion)
        {
            return await _client.GetFundings(providerFundingVersion);
        }
    }
}