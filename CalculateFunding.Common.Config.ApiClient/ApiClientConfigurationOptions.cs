using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CalculateFunding.Common.Config.ApiClient
{
    public static class ApiClientConfigurationOptions
    {
        public static void SetDefaultApiClientConfigurationOptions(HttpClient httpClient, ApiOptions options, IServiceCollection services)
        {
            Guard.ArgumentNotNull(httpClient, nameof(httpClient));
            Guard.ArgumentNotNull(options, nameof(options));
            Guard.ArgumentNotNull(services, nameof(services));

            if (string.IsNullOrWhiteSpace(options.ApiEndpoint))
            {
                throw new InvalidOperationException("options EndPoint is null or empty string");
            }

            string baseAddress = options.ApiEndpoint;
            if (!baseAddress.EndsWith("/", StringComparison.CurrentCulture))
            {
                baseAddress = $"{baseAddress}/";
            }

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            httpClient.BaseAddress = new Uri(baseAddress, UriKind.Absolute);
            httpClient.DefaultRequestHeaders?.Add(ApiClientHeaders.ApiKey, options.ApiKey);

            httpClient.DefaultRequestHeaders?.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders?.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpClient.DefaultRequestHeaders?.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
        }
    }
}
