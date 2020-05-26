using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.ApiClient.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Threading;

namespace CalculateFunding.Common.Config.ApiClient.Jobs
{
    public static class ServiceCollectionExtensions
    {
        private const string ClientName = "jobsClient";

        public static IServiceCollection AddJobsInterServiceClient(this IServiceCollection builder, IConfiguration config,
            TimeSpan[] retryTimeSpans = null, int numberOfExceptionsBeforeCircuitBreaker = 100, TimeSpan circuitBreakerFailurePeriod = default, TimeSpan handlerLifetime = default)
        {
            if (retryTimeSpans == null)
            {
                retryTimeSpans = new[] { TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5) };
            }
          

            if (circuitBreakerFailurePeriod == default(TimeSpan))
            {
                circuitBreakerFailurePeriod = TimeSpan.FromMinutes(1);
            }

            IHttpClientBuilder httpBuilder = builder.AddHttpClient(HttpClientKeys.Jobs,
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

            builder
                .AddSingleton<IJobsApiClient, JobsApiClient>();

            return builder;
        }
    }
}
