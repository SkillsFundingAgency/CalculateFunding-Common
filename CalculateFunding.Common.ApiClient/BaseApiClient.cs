using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace CalculateFunding.Common.ApiClient
{
    public abstract class BaseApiClient
    {
        private readonly string _clientKey;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        private readonly ICancellationTokenProvider _cancellationTokenProvider;

        public BaseApiClient(IHttpClientFactory httpClientFactory, string clientKey, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
        {
            Guard.ArgumentNotNull(httpClientFactory, nameof(httpClientFactory));
            Guard.IsNullOrWhiteSpace(clientKey, nameof(clientKey));
            Guard.ArgumentNotNull(logger, nameof(logger));

            _httpClientFactory = httpClientFactory;

            _clientKey = clientKey;
            Logger = logger;

            HttpClient httpClient = _httpClientFactory.CreateClient();

            Logger.Debug("AbstractApiClient created with Client Key: {clientkey} with base address: {baseAddress}", clientKey, httpClient.BaseAddress);
            _cancellationTokenProvider = cancellationTokenProvider;
        }

        protected ILogger Logger { get; }

        #region "Internal helper methods"
        private void IsOk(HttpMethod method, IEnumerable<HttpMethod> validMethods)
        {
            if (!validMethods.Contains(method)) throw new ArgumentException($"Method {method} not supported for this operation");
        }

        #region "Request"
        private async Task<HttpClient> PrepareRequest(string url, string logMessage, params object[] logParameters)
        {
            return await PrepareRequest(url, null, logMessage, logParameters);
        }

        private async Task<HttpClient> PrepareRequest(string url, TimeSpan? timeout, string logMessage, params object[] logParameters)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            HttpClient httpClient = await GetHttpClient();
            if (timeout != null) httpClient.Timeout = timeout.Value;

            Logger.Debug(logMessage, logParameters);

            return httpClient;
        }

        private async Task<ApiResponse<T>> TypedRequest<T>(string url, HttpMethod httpMethod, CancellationToken cancellationToken)
        {
            IsOk(httpMethod, new[] { HttpMethod.Get });

            HttpClient httpClient = await PrepareRequest(url,
                TimeSpan.FromMinutes(5),
                $"ApiClient {httpMethod}: {{clientKey}}://{{url}}",
                _clientKey,
                url);

            if (cancellationToken == default(CancellationToken)) cancellationToken = CurrentCancellationToken();

            using (HttpRequestMessage request = new HttpRequestMessage(httpMethod, url))
            {
                using (HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken))
                {
                    return await TypedApiResponse<T>(url, response, httpClient);
                }
            }
        }

        private async Task<HttpStatusCode> StatusCodeRequest(string url, HttpMethod httpMethod, CancellationToken cancellationToken)
        {
            IsOk(httpMethod, new[] { HttpMethod.Get, HttpMethod.Head, HttpMethod.Post });

            HttpClient httpClient = await PrepareRequest(url,
                TimeSpan.FromMinutes(5),
                $"ApiClient {httpMethod}: {{clientKey}}://{{url}}",
                _clientKey,
                url);

            if (cancellationToken == default(CancellationToken)) cancellationToken = CurrentCancellationToken();

            using (HttpRequestMessage request = new HttpRequestMessage(httpMethod, url))
            {
                return await StatusCodeResponse(url,
                    httpClient,
                    async () => await httpClient.GetAsync(url, cancellationToken));
            }
        }

        private async Task<HttpStatusCode> StatusCodeRequest<TRequest>(string url, TRequest request, HttpMethod httpMethod, CancellationToken cancellationToken)
        {
            IsOk(httpMethod, new[] { HttpMethod.Post, HttpMethod.Put });

            HttpClient httpClient = await PrepareRequest(url,
                $"ApiClient {httpMethod}: {{clientKey}}://{{url}} ({typeof(TRequest).Name})",
                _clientKey,
                url);

            if (cancellationToken == default(CancellationToken)) cancellationToken = CurrentCancellationToken();

            string json = JsonConvert.SerializeObject(request, _serializerSettings);

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(httpMethod, url))
            {
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                return await StatusCodeResponse(url,
                    httpClient,
                    async () => await httpClient.SendAsync(requestMessage, cancellationToken));
            }
        }

        private async Task<ApiResponse<TResponse>> TypedApiRequest<TResponse, TRequest>(string url, TRequest request, HttpMethod httpMethod, CancellationToken cancellationToken)
        {
            IsOk(httpMethod, new[] { HttpMethod.Post, HttpMethod.Put });

            var httpClient = await PrepareRequest(url,
                $"ApiClient {httpMethod}: {{clientKey}}://{{url}} ({typeof(TRequest).Name} => {typeof(TResponse).Name})",
                _clientKey,
                url);

            if (cancellationToken == default(CancellationToken)) cancellationToken = CurrentCancellationToken();

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(httpMethod, url))
            {
                string json = JsonConvert.SerializeObject(request, _serializerSettings);
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await httpClient.SendAsync(requestMessage, cancellationToken))
                {
                    HandleNullResponse(url, response, httpClient);

                    return await TypedApiResponse<TResponse>(url, response, httpClient);
                }
            }
        }

        private async Task<ValidatedApiResponse<TResponse>> ValidatedRequest<TResponse, TRequest>(string url,
            TRequest request,
            HttpMethod httpMethod,
            CancellationToken cancellationToken,
            TimeSpan? timeout = null)
        {
            IsOk(httpMethod, new[] { HttpMethod.Post, HttpMethod.Put });

            HttpClient httpClient = await PrepareRequest(url,
                timeout,
                $"ApiClient Validated {httpMethod}: {{clientKey}}://{{url}} ({typeof(TRequest).Name} => {typeof(TResponse).Name})",
                _clientKey,
                url);

            if (cancellationToken == default(CancellationToken)) cancellationToken = CurrentCancellationToken();

            string json = JsonConvert.SerializeObject(request, _serializerSettings);

            using (HttpRequestMessage requestMessage = new HttpRequestMessage(httpMethod, url))
            {
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
                using (HttpResponseMessage response = await httpClient.SendAsync(requestMessage, cancellationToken))
                {
                    return await ValidatedApiResponse<TResponse, TRequest>(url, response, httpClient);
                }
            }
        }
        #endregion "Request"

        #region "Response"
        private void HandleNullResponse(string url, HttpResponseMessage response, HttpClient httpClient)
        {
            if (response == null) throw new HttpRequestException($"Unable to connect to server. Url={httpClient.BaseAddress.AbsoluteUri}{url}");
        }

        private async Task<ApiResponse<T>> TypedApiResponse<T>(string url, HttpResponseMessage response, HttpClient httpClient)
        {
            HandleNullResponse(url, response, httpClient);

            if (response.IsSuccessStatusCode)
            {
                string bodyContent = await response.Content.ReadAsStringAsync();
                return new ApiResponse<T>(response.StatusCode, JsonConvert.DeserializeObject<T>(bodyContent, _serializerSettings));
            }

            return new ApiResponse<T>(response.StatusCode);
        }

        private async Task<ValidatedApiResponse<TResponse>> ValidatedApiResponse<TResponse, TRequest>(string url, HttpResponseMessage response, HttpClient httpClient)
        {
            HandleNullResponse(url, response, httpClient);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return new ValidatedApiResponse<TResponse>(response.StatusCode, JsonConvert.DeserializeObject<TResponse>(content, _serializerSettings));
            }

            ValidatedApiResponse<TResponse> apiResponse = new ValidatedApiResponse<TResponse>(response.StatusCode);

            if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                if (response.Content != null)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    apiResponse.ModelState = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<string>>>(content, _serializerSettings);
                }
            }

            return apiResponse;
        }

        private async Task<HttpStatusCode> StatusCodeResponse(string url, HttpClient httpClient, Func<Task<HttpResponseMessage>> call)
        {
            using (HttpResponseMessage response = await call())
            {
                HandleNullResponse(url, response, httpClient);

                return response.StatusCode;
            }
        }
        #endregion "Response"
        #endregion "Internal helper methods"

        #region "GET"
        public async Task<ApiResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await TypedRequest<T>(url, HttpMethod.Get, cancellationToken);
        }

        public async Task<HttpStatusCode> GetAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await StatusCodeRequest(url, HttpMethod.Get, cancellationToken);
        }
        #endregion "GET"

        #region "HEAD"
        public async Task<HttpStatusCode> HeadAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await StatusCodeRequest(url, HttpMethod.Head, cancellationToken);
        }
        #endregion "HEAD"

        #region "POST"
        public async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(string url,
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await TypedApiRequest<TResponse, TRequest>(url, request, HttpMethod.Post, cancellationToken);
        }

        public async Task<HttpStatusCode> PostAsync<TRequest>(string url,
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await StatusCodeRequest(url, request, HttpMethod.Post, cancellationToken);
        }

        public async Task<ValidatedApiResponse<TResponse>> ValidatedPostAsync<TResponse, TRequest>(string url,
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken),
            TimeSpan? timeout = null)
        {
            return await ValidatedRequest<TResponse, TRequest>(url, request, HttpMethod.Post, cancellationToken, timeout);
        }

        public async Task<NoValidatedContentApiResponse> ValidatedPostAsync<TRequest>(string url,
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken),
            TimeSpan? timeout = null)
        {
            HttpClient httpClient = await PrepareRequest(url,
                timeout,
                $"ApiClient Validated POST: {{clientKey}}://{{url}} ({typeof(TRequest).Name})",
                _clientKey,
                url);

            if (cancellationToken == default(CancellationToken)) cancellationToken = CurrentCancellationToken();

            string json = JsonConvert.SerializeObject(request, _serializerSettings);

            using (HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken))
            {
                HandleNullResponse(url, response, httpClient);

                if (response.IsSuccessStatusCode) return new NoValidatedContentApiResponse(response.StatusCode);

                NoValidatedContentApiResponse apiResponse = new NoValidatedContentApiResponse(response.StatusCode);

                if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    if (response.Content != null)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        apiResponse.ModelState = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<string>>>(content, _serializerSettings);
                    }
                }

                return apiResponse;
            }
        }

        public async Task<HttpStatusCode> PostAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await StatusCodeRequest(url, HttpMethod.Post, cancellationToken);
        }
        #endregion

        #region "PUT"
        public async Task<ApiResponse<TResponse>> PutAsync<TResponse, TRequest>(string url,
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await TypedApiRequest<TResponse, TRequest>(url, request, HttpMethod.Put, cancellationToken);
        }

        public async Task<HttpStatusCode> PutAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await StatusCodeRequest(url, request, HttpMethod.Put, cancellationToken);
        }

        public async Task<ValidatedApiResponse<TResponse>> ValidatedPutAsync<TResponse, TRequest>(string url, TRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ValidatedRequest<TResponse, TRequest>(url, request, HttpMethod.Put, cancellationToken);
        }
        #endregion "PUT"

        protected HttpClient CreateHttpClient()
        {
            return _httpClientFactory.CreateClient(_clientKey);
        }

        public virtual async Task<HttpClient> GetHttpClient()
        {
            return await Task.FromResult(_httpClientFactory.CreateClient(_clientKey));
        }

        private CancellationToken CurrentCancellationToken()
        {
            if (_cancellationTokenProvider != null) return _cancellationTokenProvider.CurrentCancellationToken();

            return default(CancellationToken);
        }
    }
}
