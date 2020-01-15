using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.ApiClient.Results;
using CalculateFunding.Common.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CalculateFunding.Common.Config.ApiClient.Results
{
    public static class ServiceCollectionExtensions
    {       
        public static IServiceCollection AddResultsInterServiceClient(this IServiceCollection builder, IConfiguration config,
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

            builder.AddHttpClient(HttpClientKeys.Results,
               c =>
               {
                   ApiOptions apiOptions = new ApiOptions();
                   
                   config.Bind("resultsClient", apiOptions);

                   ApiClientConfigurationOptions.SetDefaultApiClientConfigurationOptions(c, apiOptions, builder);
               })
               .ConfigurePrimaryHttpMessageHandler(() => new ApiClientHandler())
               .AddTransientHttpErrorPolicy(c => c.WaitAndRetryAsync(retryTimeSpans))
               .AddTransientHttpErrorPolicy(c => c.CircuitBreakerAsync(numberOfExceptionsBeforeCircuitBreaker, circuitBreakerFailurePeriod));

            builder
                .AddSingleton<IResultsApiClient, ResultsApiClient>();

            return builder;
        }

       
    }
}
