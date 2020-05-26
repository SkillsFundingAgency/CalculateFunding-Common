using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.Models;
using CalculateFunding.Common.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CalculateFunding.Common.Config.ApiClient
{
    public static class ApiClientConfigurationOptions
    {
        public static void SetDefaultApiClientConfigurationOptions(HttpClient httpClient, ApiOptions options)
        {
            Guard.ArgumentNotNull(httpClient, nameof(httpClient));
            Guard.ArgumentNotNull(options, nameof(options));
            
            if (string.IsNullOrWhiteSpace(options.ApiEndpoint))
            {
                throw new InvalidOperationException("options EndPoint is null or empty string");
            }

            string baseAddress = options.ApiEndpoint;
            if (!baseAddress.EndsWith("/", StringComparison.CurrentCulture))
            {
                baseAddress = $"{baseAddress}/";
            }

            httpClient.BaseAddress = new Uri(baseAddress, UriKind.Absolute);
            if (httpClient.DefaultRequestHeaders != null)
            {
                httpClient.DefaultRequestHeaders.Add(ApiClientHeaders.ApiKey, options.ApiKey);
                
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            }
        }

        public static IHttpClientBuilder AddUserProfilerHeaderPropagation(this IHttpClientBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddHttpMessageHandler(services =>
            {
                return new UserProfilerPropagationMessageHandler(services.GetRequiredService<IUserProfileProvider>());
            });

            return builder;
        }
    }
}
