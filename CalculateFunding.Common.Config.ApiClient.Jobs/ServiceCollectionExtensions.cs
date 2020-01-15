using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.ApiClient.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;


namespace CalculateFunding.Common.Config.ApiClient.Jobs
{
    public static class ServiceCollectionExtensions
    {      
        public static IServiceCollection AddJobsInterServiceClient(this IServiceCollection builder, IConfiguration config,
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

            builder.AddHttpClient(HttpClientKeys.Jobs,
               c =>
               {
                   ApiOptions apiOptions = new ApiOptions();

                   config.Bind("jobsClient", apiOptions);

                   ApiClientConfigurationOptions.SetDefaultApiClientConfigurationOptions(c, apiOptions, builder);
               })
               .ConfigurePrimaryHttpMessageHandler(() => new ApiClientHandler())
               .AddTransientHttpErrorPolicy(c => c.WaitAndRetryAsync(retryTimeSpans))
               .AddTransientHttpErrorPolicy(c => c.CircuitBreakerAsync(numberOfExceptionsBeforeCircuitBreaker, circuitBreakerFailurePeriod));

            builder
                .AddSingleton<IJobsApiClient, JobsApiClient>();

            return builder;
        }
    }
}
