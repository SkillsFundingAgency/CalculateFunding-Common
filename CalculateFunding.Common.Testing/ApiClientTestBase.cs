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

        protected string NewRandomString()
        {
            return new RandomString();
        }

        protected void GivenTheResponse<TResponse>(TResponse response)
            where TResponse : class
        {
            _messageHandler.SetupStringResponse(response.AsJson());
        }

        protected void GivenTheStatusCode(HttpStatusCode statusCode)
        {
            _messageHandler.SetupStatusCodeResponse(statusCode);    
        }
    }
}