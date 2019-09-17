using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace CalculateFunding.Common.Testing
{
    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly ICollection<QueuedResponse> _queuedResponses = new List<QueuedResponse>();
        private readonly ICollection<ReceivedRequest> _requests = new List<ReceivedRequest>();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            string uri = request.RequestUri.ToString();

            _requests.Add(new ReceivedRequest(uri, request.Content?.ReadAsStringAsync()
                .GetAwaiter()
                .GetResult()));

            HttpResponseMessage httpResponseMessage = _queuedResponses.FirstOrDefault(_ => _.IsForRequest(request))?.Response;

            httpResponseMessage
                .Should()
                .NotBeNull("Expected a queued response for uri {0} method {1}", uri, request.Method?.ToString());

            return Task.FromResult(httpResponseMessage);
        }

        public void SetupStatusCodeResponse(string uri, HttpStatusCode statusCode, HttpMethod method = null, params string[] customerHeaders)
        {
            SetupStringResponse(uri, null, statusCode, method, customerHeaders);
        }

        public void SetupStringResponse(string uri, string responseContent,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            HttpMethod method = null,
            params string[] customerHeaders)
        {
            _queuedResponses.Add(new QueuedResponse(new HttpResponseMessage(statusCode)
            {
                Content = responseContent == null ? null : new StringContent(responseContent)
            }, uri, method, customerHeaders));
        }

        public IEnumerable<string> RequestedUris => _requests.Select(_ => _.Uri);

        public IEnumerable<string> RequestContents => _requests.Select(_ => _.Content);
        
        private class ReceivedRequest
        {
            public ReceivedRequest(string uri, string content)
            {
                Uri = uri;
                Content = content;
            }

            public string Uri { get; }
            
            public string Content { get; }
        }
        
        private class QueuedResponse
        {
            private readonly string _uri;
            private readonly HttpMethod _method;
            private readonly ICollection<KeyValuePair<string, IEnumerable<string>>> _expectedIncludedHeaders = new List<KeyValuePair<string, IEnumerable<string>>>();

            public QueuedResponse(HttpResponseMessage response, string uri, HttpMethod method, string[] customerHeaders)
            {
                Response = response;
                _uri = uri;
                _method = method;
                
                (customerHeaders.Length % 2)
                    .Should()
                    .Be(0, "Expected headers must be supplied in name value pairs");
                
                for (int headerName = 0; headerName < customerHeaders.Length; headerName += 2)
                {
                    _expectedIncludedHeaders.Add(new KeyValuePair<string, IEnumerable<string>>(customerHeaders[headerName], new[] {customerHeaders[headerName + 1]}));
                }
            }

            public HttpResponseMessage Response { get; }

            public bool IsForRequest(HttpRequestMessage requestMessage)
            {
                return requestMessage.RequestUri.ToString().EndsWith(_uri) &&
                       (_method == null || _method.Equals(requestMessage.Method)) &&
                     _expectedIncludedHeaders.All(_ => requestMessage.Headers.TryGetValues(_.Key, out IEnumerable<string> values) &&
                                                       _.Value.SequenceEqual(values));
            }
        }
    }
}