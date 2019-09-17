using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CalculateFunding.Common.Testing
{
    public abstract class ApiClientTestBase
    {
        protected IHttpClientFactory ClientFactory;

        private const string HttpStubUri = "http://stuburi";
        private HttpMessageHandlerStub _messageHandler;

        [TestInitialize]
        public void ApiClientTestBaseSetUp()
        {
            _messageHandler = new HttpMessageHandlerStub();

            ClientFactory = Substitute.For<IHttpClientFactory>();

            ClientFactory.CreateClient(Arg.Any<string>())
                .Returns(new HttpClient(_messageHandler)
                {
                    BaseAddress = new Uri(HttpStubUri)
                });
        }

        protected void AndTheUrisShouldHaveBeenRequested(params string[] expectedUris)
        {
            _messageHandler.RequestedUris
                .Should()
                .BeEquivalentTo(expectedUris.Select(uri => $"{HttpStubUri}/{uri}"));
        }

        protected void AndTheRequestContentsShouldHaveBeen(params string[] expectedRequestBodies)
        {
            _messageHandler.RequestContents
                .Should()
                .BeEquivalentTo(expectedRequestBodies);
        }

        protected string NewRandomString()
        {
            return new RandomString();
        }

        protected void GivenTheResponse<TResponse>(string uri, TResponse response, HttpMethod method = null, params string[] customHeaders)
            where TResponse : class
        {
            _messageHandler.SetupStringResponse(uri, response.AsJson(), method: method, customerHeaders: customHeaders);
        }

        protected void GivenTheStatusCode(string uri, HttpStatusCode statusCode, HttpMethod method = null, params string[] customHeaders)
        {
            _messageHandler.SetupStatusCodeResponse(uri, statusCode, method, customHeaders);
        }
    }
}