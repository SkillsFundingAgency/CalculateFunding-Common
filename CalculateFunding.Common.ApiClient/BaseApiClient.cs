namespace CalculateFunding.Common.ApiClient
{
    using System;
    using System.Collections.Generic;
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

    public abstract class BaseApiClient
    {
        private readonly ILogger _logger;
        private readonly string _clientKey;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() };
        private readonly ICancellationTokenProvider _cancellationTokenProvider;

        public BaseApiClient(IHttpClientFactory httpClientFactory, string clientKey, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
        {
            Guard.ArgumentNotNull(httpClientFactory, nameof(httpClientFactory));
            Guard.IsNullOrWhiteSpace(clientKey, nameof(clientKey));
            Guard.ArgumentNotNull(logger, nameof(logger));

            _httpClientFactory = httpClientFactory;

            _clientKey = clientKey;
            _logger = logger;

            HttpClient httpClient = GetHttpClient();

            _logger.Debug("AbstractApiClient created with Client Key: {clientkey} with base address: {baseAddress}", clientKey, httpClient.BaseAddress);
            _cancellationTokenProvider = cancellationTokenProvider;
        }

        protected ILogger Logger
        {
            get
            {
                return _logger;
            }
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            HttpClient httpClient = GetHttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(5);

            _logger.Debug("ApiClient GET: {clientKey}://{url}", _clientKey, url);

            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CurrentCancellationToken();
            }

            using (HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken))
            {
                if (response == null)
                {
                    throw new HttpRequestException($"Unable to connect to server. Url={httpClient.BaseAddress.AbsoluteUri}{url}");
                }

                if (response.IsSuccessStatusCode)
                {
                    string bodyContent = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<T>(response.StatusCode, JsonConvert.DeserializeObject<T>(bodyContent, _serializerSettings));
                }

                return new ApiResponse<T>(response.StatusCode);
            }
        }

        public async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(string url, TRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            HttpClient httpClient = GetHttpClient();

            string json = JsonConvert.SerializeObject(request, _serializerSettings);
            _logger.Debug($"ApiClient POST: {{clientKey}}://{{url}} ({typeof(TRequest).Name} => {typeof(TResponse).Name})", _clientKey, url);

            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CurrentCancellationToken();
            }

            using (HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken))
            {
                if (response == null)
                {
                    throw new HttpRequestException($"Unable to connect to server. Url={httpClient.BaseAddress.AbsoluteUri}{url}");
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return new ApiResponse<TResponse>(response.StatusCode, JsonConvert.DeserializeObject<TResponse>(responseBody, _serializerSettings));
                }

                return new ApiResponse<TResponse>(response.StatusCode);
            }
        }

        public async Task<ValidatedApiResponse<TResponse>> ValidatedPostAsync<TResponse, TRequest>(string url, TRequest request, CancellationToken cancellationToken = default(CancellationToken), TimeSpan? timeout = null)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            HttpClient httpClient = GetHttpClient();
            if (timeout.HasValue)
            {
                httpClient.Timeout = timeout.Value;
            }

            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CurrentCancellationToken();
            }

            string json = JsonConvert.SerializeObject(request, _serializerSettings);
            _logger.Debug($"ApiClient Validated POST: {{clientKey}}://{{url}} ({typeof(TRequest).Name} => {typeof(TResponse).Name})", _clientKey, url);
            using (HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken))
            {
                if (response == null)
                {
                    throw new HttpRequestException($"Unable to connect to server. Url={httpClient.BaseAddress.AbsoluteUri}{url}");
                }

                if (response.IsSuccessStatusCode)
                {
                    return new ValidatedApiResponse<TResponse>(response.StatusCode, JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync(), _serializerSettings));
                }

                ValidatedApiResponse<TResponse> apiResponse = new ValidatedApiResponse<TResponse>(response.StatusCode);

                if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    apiResponse.ModelState = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<string>>>(await response.Content.ReadAsStringAsync(), _serializerSettings);

                }

                return apiResponse;
            }
        }

        public async Task<HttpStatusCode> PostAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            HttpClient httpClient = GetHttpClient();

            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CurrentCancellationToken();
            }

            string json = JsonConvert.SerializeObject(request, _serializerSettings);
            _logger.Debug($"ApiClient POST: {{clientKey}}://{{url}} ({typeof(TRequest).Name})", _clientKey, url);
            using (HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken))
            {
                if (response == null)
                {
                    throw new HttpRequestException($"Unable to connect to server. Url={httpClient.BaseAddress.AbsoluteUri}{url}");
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> PostAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            HttpClient httpClient = GetHttpClient();

            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CurrentCancellationToken();
            }

            _logger.Debug($"ApiClient POST: {{clientKey}}://{{url}}", _clientKey, url);
            using (HttpResponseMessage response = await httpClient.PostAsync(url, null, cancellationToken))
            {
                if (response == null)
                {
                    throw new HttpRequestException($"Unable to connect to server. Url={httpClient.BaseAddress.AbsoluteUri}{url}");
                }

                return response.StatusCode;
            }
        }

        public async Task<HttpStatusCode> PutAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            HttpClient httpClient = GetHttpClient();

            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CurrentCancellationToken();
            }

            string json = JsonConvert.SerializeObject(request, _serializerSettings);
            _logger.Debug($"ApiClient PUT: {{clientKey}}://{{url}} ({typeof(TRequest).Name})", _clientKey, url);
            using (HttpResponseMessage response = await httpClient.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken))
            {
                if (response == null)
                {
                    throw new HttpRequestException($"Unable to connect to server. Url={httpClient.BaseAddress.AbsoluteUri}{url}");
                }

                return response.StatusCode;
            }
        }

        public async Task<ValidatedApiResponse<TResponse>> ValidatedPutAsync<TResponse, TRequest>(string url, TRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            HttpClient httpClient = GetHttpClient();

            if (cancellationToken == default(CancellationToken))
            {
                cancellationToken = CurrentCancellationToken();
            }

            string json = JsonConvert.SerializeObject(request, _serializerSettings);

            _logger.Debug($"ApiClient Validated POST: {{clientKey}}://{{url}} ({typeof(TRequest).Name} => {typeof(TResponse).Name})", _clientKey, url);

            using (HttpResponseMessage response = await httpClient.PutAsync(url, new StringContent(json, Encoding.UTF8, "application/json"), cancellationToken))
            {
                if (response == null)
                {
                    throw new HttpRequestException($"Unable to connect to server. Url={httpClient.BaseAddress.AbsoluteUri}{url}");
                }

                if (response.IsSuccessStatusCode)
                {
                    return new ValidatedApiResponse<TResponse>(response.StatusCode, JsonConvert.DeserializeObject<TResponse>(await response.Content.ReadAsStringAsync(), _serializerSettings));
                }

                ValidatedApiResponse<TResponse> apiResponse = new ValidatedApiResponse<TResponse>(response.StatusCode);

                if (apiResponse.StatusCode == HttpStatusCode.BadRequest)
                {
                    apiResponse.ModelState = JsonConvert.DeserializeObject<IDictionary<string, IEnumerable<string>>>(await response.Content.ReadAsStringAsync(), _serializerSettings);
                }

                return apiResponse;
            }
        }

        private HttpClient GetHttpClient()
        {

            return _httpClientFactory.CreateClient(_clientKey);
        }

        private CancellationToken CurrentCancellationToken()
        {
            if (_cancellationTokenProvider != null)
            {
                return _cancellationTokenProvider.CurrentCancellationToken();
            }

            return default(CancellationToken);
        }
    }
}
