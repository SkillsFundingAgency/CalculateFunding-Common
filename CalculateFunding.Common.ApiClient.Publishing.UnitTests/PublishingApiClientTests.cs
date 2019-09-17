using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        [DataRow(HttpStatusCode.OK)]
        [DataRow(HttpStatusCode.Unauthorized)]
        [DataRow(HttpStatusCode.InsufficientStorage)]
        public async Task RefreshFundingForSpecificationPostsToPublishingService(HttpStatusCode expectedStatusCode)
        {
            string specificationId = NewRandomString();
            string expectedUri = $"specifications/{specificationId}/refresh";

            GivenTheStatusCode(expectedUri, expectedStatusCode, HttpMethod.Post);

            HttpStatusCode actualStatusCode = await _client.RefreshFundingForSpecification(specificationId);

            actualStatusCode
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

            HttpStatusCode actualStatusCode = await _client.ApproveSpecification(specificationId);

            actualStatusCode
                .Should()
                .Be(expectedStatusCode);

            AndTheUrisShouldHaveBeenRequested(expectedUri);
        }
    }
}