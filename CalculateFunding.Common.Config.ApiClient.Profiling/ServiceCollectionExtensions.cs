using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.ApiClient.Bearer;
using CalculateFunding.Common.ApiClient.Profiling;
using CalculateFunding.Common.Caching;
using CalculateFunding.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Serilog;
using System;
using System.Net.Http;
using System.Threading;

namespace CalculateFunding.Common.Config.ApiClient.Profiling
{
    public static class ServiceCollectionExtensions
    {
        private const string ClientName = "providerProfilingClient";

        public static IServiceCollection AddProfilingInterServiceClient(this IServiceCollection builder, IConfiguration config,
            TimeSpan[] retryTimeSpans = null, int numberOfExceptionsBeforeCircuitBreaker = 100, TimeSpan circuitBreakerFailurePeriod = default, TimeSpan handlerLifetime = default)
        {
            if (retryTimeSpans == null)
            {
                retryTimeSpans = new[] { TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5) };
            }

            if (circuitBreakerFailurePeriod == default)
            {
                circuitBreakerFailurePeriod = TimeSpan.FromMinutes(1);
            }

            var httpBuilder = builder.AddHttpClient(HttpClientKeys.Profiling,
               (httpClient) =>
               {
                   ApiOptions apiOptions = new ApiOptions();

                   config.Bind(ClientName, apiOptions);

                   ApiClientConfigurationOptions.SetDefaultApiClientConfigurationOptions(httpClient, apiOptions);
               })
               .ConfigurePrimaryHttpMessageHandler(() => new ApiClientHandler())
               .AddTransientHttpErrorPolicy(c => c.WaitAndRetryAsync(retryTimeSpans))
               .AddTransientHttpErrorPolicy(c => c.CircuitBreakerAsync(numberOfExceptionsBeforeCircuitBreaker, circuitBreakerFailurePeriod))
               .AddUserProfilerHeaderPropagation();

            // if a life time for the handler has been set then set it on the client builder
            if (handlerLifetime != default)
            {
                httpBuilder.SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            }

            builder.AddSingleton<ICancellationTokenProvider, InactiveCancellationTokenProvider>();

            builder.AddSingleton<IAzureBearerTokenProxy, AzureBearerTokenProxy>();

            builder.AddSingleton<IProfilingApiClient>((ctx) =>
            {
                IHttpClientFactory httpClientFactory = ctx.GetService<IHttpClientFactory>();
                ILogger logger = ctx.GetService<ILogger>();
                ICancellationTokenProvider cancellationTokenProvider = ctx.GetService<ICancellationTokenProvider>();

                IAzureBearerTokenProxy azureBearerTokenProxy = ctx.GetService<IAzureBearerTokenProxy>();
                ICacheProvider cacheProvider = ctx.GetService<ICacheProvider>();

                AzureBearerTokenOptions azureBearerTokenOptions = new AzureBearerTokenOptions();
                config.Bind("providerProfilingAzureBearerTokenOptions", azureBearerTokenOptions);

                AzureBearerTokenProvider bearerTokenProvider = new AzureBearerTokenProvider(azureBearerTokenProxy, cacheProvider, azureBearerTokenOptions);

                return new ProfilingApiClient(httpClientFactory, HttpClientKeys.Profiling, logger, bearerTokenProvider, cancellationTokenProvider);
            });

            return builder;
        }
    }
}
