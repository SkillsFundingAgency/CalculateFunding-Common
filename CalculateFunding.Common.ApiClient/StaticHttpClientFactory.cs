using System;
using System.Collections.Generic;
using System.Net.Http;
using CalculateFunding.Common.Utility;

namespace CalculateFunding.Common.ApiClient
{
    public class StaticHttpClientFactory : IHttpClientFactory, IDisposable
    {
        readonly Dictionary<string, Func<HttpClient>> _httpClients = new Dictionary<string, Func<HttpClient>>();

        readonly List<HttpClient> _createdHttpClients = new List<HttpClient>();

        public HttpClient CreateClient(string name)
        {
            Guard.IsNullOrWhiteSpace(name, nameof(name));

            if (!_httpClients.ContainsKey(name))
            {
                throw new InvalidOperationException($"Unable to find registered client with name of '{name}'");
            }

            HttpClient client = _httpClients[name].Invoke();
            _createdHttpClients.Add(client);

            return client;
        }

        public Func<HttpClient> AddClient(string name, Func<HttpClient> httpClient)
        {
            Guard.IsNullOrWhiteSpace(name, nameof(name));
            Guard.ArgumentNotNull(httpClient, nameof(httpClient));

            _httpClients.Add(name, httpClient);

            return httpClient;
        }

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (HttpClient client in _createdHttpClients)
                    {
                        if (client != null)
                        {
                            client.Dispose();
                        }
                    }
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StaticHttpClientFactory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
    }
}
