using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.ApiClient.Policies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Threading;

namespace CalculateFunding.Common.Config.ApiClient.Policies
{
    public static class ServiceCollectionExtensions
    {
        private const string ClientName = "policiesClient";

        public static IServiceCollection AddPoliciesInterServiceClient(this IServiceCollection builder, IConfiguration config,
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

            IHttpClientBuilder httpBuilder = builder.AddHttpClient(HttpClientKeys.Policies,
               c =>
               {
                   ApiOptions apiOptions = new ApiOptions();

                   config.Bind(ClientName, apiOptions);

                   ApiClientConfigurationOptions.SetDefaultApiClientConfigurationOptions(c, apiOptions, builder, ClientName);
               })
               .ConfigurePrimaryHttpMessageHandler(() => new ApiClientHandler())
               .AddTransientHttpErrorPolicy(c => c.WaitAndRetryAsync(retryTimeSpans))
               .AddTransientHttpErrorPolicy(c => c.CircuitBreakerAsync(numberOfExceptionsBeforeCircuitBreaker, circuitBreakerFailurePeriod));

            // if a life time for the handler has been set then set it on the client builder
            if (handlerLifetime != default)
            {
                httpBuilder.SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            }

            builder
                .AddSingleton<IPoliciesApiClient, PoliciesApiClient>();

            return builder;
        }
    }
}
