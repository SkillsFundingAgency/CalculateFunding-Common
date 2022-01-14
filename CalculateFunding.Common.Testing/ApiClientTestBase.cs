using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Bearer;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CalculateFunding.Common.Testing
{
    public abstract class ApiClientTestBase
    {
        protected IHttpClientFactory ClientFactory;
        protected IBearerTokenProvider BearerTokenProvider;

        private string _bearerToken;

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

            _bearerToken = NewRandomString();

            BearerTokenProvider = Substitute.For<IBearerTokenProvider>();

            BearerTokenProvider.GetToken()
                .Returns(_bearerToken);
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

        protected DateTime NewRandomDateTime()
        {
            return new RandomDateTime();
        }

        protected string NewRandomHeaderValue() => $"\"{NewRandomString().Replace("-", "")}\"";

        protected int NewRandomInt()
        {
            return new Random().Next();
        }

        protected bool NewRandomBoolean()
        {
            return new RandomBoolean();
        }

        protected void GivenThePrimitiveResponse<TResponse>(string uri, TResponse response, HttpMethod method = null, params string[] customHeaders)
        {
            _messageHandler.SetupStringResponse(uri, response.ToString().ToLower(), method: method, customerHeaders: customHeaders);
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

        protected async Task AssertGetRequest<TResponse, TApiResponse>(string expectedUri,
            TResponse expectedResponse,
            Func<Task<TApiResponse>> action,
            params string[] customHeaders)
            where TResponse : class
            where TApiResponse : ApiResponse<TResponse>
        {
            await AssertRequest(expectedUri,
                expectedResponse,
                HttpMethod.Get,
                action);
        }

        protected async Task AssertGetRequest<TRequest, TResponse, TApiResponse>(string expectedUri,
            TRequest request,
            TResponse expectedResponse,
            Func<TRequest, Task<TApiResponse>> action,
            params string[] customHeaders)
            where TRequest : class
            where TResponse : class
            where TApiResponse : ApiResponse<TResponse>
        {
            await AssertRequest(expectedUri,
                request,
                expectedResponse,
                HttpMethod.Get,
                action,
                customHeaders);
        }

        protected async Task AssertGetRequest(string expectedUri,
            HttpStatusCode expectedStatusCode,
            Func<Task<HttpStatusCode>> action)
        {
            await AssertRequest(expectedUri,
                expectedStatusCode,
                action,
                HttpMethod.Get);
        }

        protected async Task AssertPostRequest<TRequest, TResponse, TApiResponse>(string expectedUri,
            TRequest request,
            TResponse expectedResponse,
            Func<TRequest, Task<TApiResponse>> action)
            where TRequest : class
            where TResponse : class
            where TApiResponse : ApiResponse<TResponse>
        {
            await AssertRequest(expectedUri,
                request,
                expectedResponse,
                HttpMethod.Post,
                action);

            AndTheRequestContentsShouldHaveBeen(request.AsJson());
        }

        protected async Task AssertPutRequest<TRequest, TResponse, TApiResponse>(string expectedUri,
            TRequest request,
            TResponse expectedResponse,
            Func<Task<TApiResponse>> action)
            where TRequest : class
            where TResponse : class
            where TApiResponse : ApiResponse<TResponse>
        {
            await AssertRequest(expectedUri,
                expectedResponse,
                HttpMethod.Put,
                action);

            AndTheRequestContentsShouldHaveBeen(request.AsJson());
        }

        protected async Task AssertPutRequest<TRequest, TApiResponse>(string expectedUri,
            TRequest request,
            HttpStatusCode expectedResponse,
            Func<Task<TApiResponse>> action)
            where TRequest : class
            where TApiResponse : ApiResponse<HttpStatusCode>
        {
            await AssertRequest(expectedUri,
                expectedResponse,
                HttpMethod.Put,
                action);

            AndTheRequestContentsShouldHaveBeen(request.AsJson());
        }

        protected async Task AssertPutRequest<TRequest>(string expectedUri,
            TRequest request,
            HttpStatusCode expectedStatusCode,
            Func<Task<HttpStatusCode>> action)
            where TRequest : class
        {
            await AssertRequest(expectedUri,
                expectedStatusCode,
                action,
                HttpMethod.Put);

            AndTheRequestContentsShouldHaveBeen(request.AsJson());
        }

        protected async Task AssertPutRequest(string expectedUri,
            HttpStatusCode expectedStatusCode,
            Func<Task<HttpStatusCode>> action)
        {
            await AssertRequest(expectedUri,
                expectedStatusCode,
                action,
                HttpMethod.Put);
        }

        protected async Task AssertPatchRequest(string expectedUri,
            HttpStatusCode expectedStatusCode,
            Func<Task<HttpStatusCode>> action)
        {
            await AssertRequest(expectedUri,
                expectedStatusCode,
                action,
                HttpMethod.Patch);
        }

        protected async Task AssertPatchRequest<TRequest, TApiResponse>(string expectedUri,
            TRequest request,
            HttpStatusCode expectedResponse,
            Func<Task<TApiResponse>> action)
            where TRequest : class
            where TApiResponse : ApiResponse<HttpStatusCode>
        {
            await AssertRequest(expectedUri,
                expectedResponse,
                HttpMethod.Patch,
                action);

            AndTheRequestContentsShouldHaveBeen(request.AsJson());
        }

        protected async Task AssertHeadRequest(string expectedUri,
            HttpStatusCode expectedStatusCode,
            Func<Task<HttpStatusCode>> action)
        {
            await AssertRequest(expectedUri,
                expectedStatusCode,
                action,
                HttpMethod.Head);
        }
        
        protected async Task AssertPostRequest<TRequest, TResponse, TApiResponse>(string expectedUri,
            TRequest request,
            TResponse expectedResponse,
            Func<Task<TApiResponse>> action)
            where TRequest : class
            where TResponse : class
            where TApiResponse : ApiResponse<TResponse>
        {
            GivenTheResponse(expectedUri, expectedResponse, HttpMethod.Post);

            TApiResponse apiResponse = await action();

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse?.Content
                .Should()
                .BeEquivalentTo(expectedResponse);

            AndTheRequestContentsShouldHaveBeen(request.AsJson());
        }

        protected async Task AssertDeleteRequest<TRequest>(string expectedUri,
            TRequest request,
            HttpStatusCode expectedStatusCode,
            Func<TRequest, Task<HttpStatusCode>> action)
            where TRequest : class
        {
            await AssertRequest(expectedUri,
                request,
                expectedStatusCode,
                action,
                HttpMethod.Delete);
        }
        
        protected async Task AssertDeleteRequest(string expectedUri,
            HttpStatusCode expectedStatusCode,
            Func<Task<HttpStatusCode>> action)
        {
            await AssertRequest(expectedUri,
                expectedStatusCode,
                action,
                HttpMethod.Delete);
        }

        protected async Task AssertPostRequest<TRequest>(string expectedUri,
            TRequest request,
            HttpStatusCode expectedStatusCode,
            Func<TRequest, Task<HttpStatusCode>> action)
            where TRequest : class
        {
            await AssertRequest(expectedUri,
                request,
                expectedStatusCode,
                action,
                HttpMethod.Post);

            AndTheRequestContentsShouldHaveBeen(request.AsJson());
        }
        
        protected async Task AssertPostRequest(string expectedUri,           
            HttpStatusCode expectedStatusCode,
            Func<Task<HttpStatusCode>> action)
        {
            await AssertRequest(expectedUri,
                expectedStatusCode,
                action,
                HttpMethod.Post);
        }

        private async Task AssertRequest(string expectedUri,
            HttpStatusCode expectedStatusCode,
            Func<Task<HttpStatusCode>> action,
            HttpMethod verb)
        {
            GivenTheStatusCode(expectedUri, expectedStatusCode, verb);

            HttpStatusCode apiResponse = await action();

            apiResponse
                .Should()
                .Be(expectedStatusCode);
        }

        private async Task AssertRequest<TRequest>(string expectedUri,
            TRequest request,
            HttpStatusCode expectedStatusCode,
            Func<TRequest, Task<HttpStatusCode>> action,
            HttpMethod verb)
            where TRequest : class
        {
            GivenTheStatusCode(expectedUri, expectedStatusCode, verb);

            HttpStatusCode apiResponse = await action(request);

            apiResponse
                .Should()
                .Be(expectedStatusCode);
        }

        private async Task AssertRequest<TRequest, TResponse, TApiResponse>(string expectedUri,
            TRequest request,
            TResponse expectedResponse,
            HttpMethod method,
            Func<TRequest, Task<TApiResponse>> action,
            params string[] customHeaders)
            where TRequest : class
            where TResponse : class
            where TApiResponse : ApiResponse<TResponse>
        {
            GivenTheResponse(expectedUri, expectedResponse, method, customHeaders);

            TApiResponse apiResponse = await action(request);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse?.Content
                .Should()
                .BeEquivalentTo(expectedResponse);
        }

        private async Task AssertRequest<TResponse, TApiResponse>(string expectedUri,
            TResponse expectedResponse,
            HttpMethod method,
            Func<Task<TApiResponse>> action,
            params string[] customHeaders)
            where TResponse : class
            where TApiResponse : ApiResponse<TResponse>
        {
            GivenTheResponse(expectedUri, expectedResponse, method, customHeaders);

            TApiResponse apiResponse = await action();

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse?.Content
                .Should()
                .BeEquivalentTo(expectedResponse);
        }

        private async Task AssertRequest<TApiResponse>(string expectedUri,
            HttpStatusCode expectedResponse,
            HttpMethod method,
            Func<Task<TApiResponse>> action,
            params string[] customHeaders)
            where TApiResponse : ApiResponse<HttpStatusCode>
        {
            GivenTheStatusCode(expectedUri, expectedResponse, method, customHeaders);

            TApiResponse apiResponse = await action();

            apiResponse?.StatusCode
                .Should()
                .BeEquivalentTo(expectedResponse);
        }

        protected IEnumerable<TItem> NewEnumerable<TItem>(params TItem[] items) => items;

        protected TModel NewModel<TModel>() where TModel : new() => new TModel();

        protected SearchModel NewRandomSearch() => new SearchModel
        {
            SearchTerm = NewRandomString()
        };
    }
}