﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Specifications.UnitTests
{
    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new Queue<HttpResponseMessage>();
        private readonly List<string> _requestedUris = new List<string>();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _requestedUris.Add(request.RequestUri.ToString());

            return Task.FromResult(_responses.Dequeue());
        }

        public void SetupStringResponse(string uri,
            string responseContent,
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responses.Enqueue(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseContent)
            });
        }

        public IEnumerable<string> RequestedUris => _requestedUris.AsReadOnly();
    }
}