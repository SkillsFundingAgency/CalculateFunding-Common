using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.ApiClient.Graph;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;

namespace CalculateFunding.Common.Config.ApiClient.Graph
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGraphInterServiceClient(this IServiceCollection builder, IConfiguration config,
            TimeSpan[] retryTimeSpans = null, int numberOfExceptionsBeforeCircuitBreaker = 100, TimeSpan circuitBreakerFailurePeriod = default(TimeSpan))
        {
            if (retryTimeSpans == null)
            {
                retryTimeSpans = new[] { TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5) };
            }

            if (circuitBreakerFailurePeriod == default(TimeSpan))
            {
                circuitBreakerFailurePeriod = TimeSpan.FromMinutes(1);
            }

            builder.AddHttpClient(HttpClientKeys.Graph,
               c =>
               {
                   ApiOptions apiOptions = new ApiOptions();

                   config.Bind("graphClient", apiOptions);

                   ApiClientConfigurationOptions.SetDefaultApiClientConfigurationOptions(c, apiOptions, builder);
               })
               .ConfigurePrimaryHttpMessageHandler(() => new ApiClientHandler())
               .AddTransientHttpErrorPolicy(c => c.WaitAndRetryAsync(retryTimeSpans))
               .AddTransientHttpErrorPolicy(c => c.CircuitBreakerAsync(numberOfExceptionsBeforeCircuitBreaker, circuitBreakerFailurePeriod));

            builder
                .AddSingleton<IGraphApiClient, GraphApiClient>();

            return builder;
        }


    }
}
