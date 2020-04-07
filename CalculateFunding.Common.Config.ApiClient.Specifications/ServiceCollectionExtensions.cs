using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.ApiClient.Specifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;

namespace CalculateFunding.Common.Config.ApiClient.Specifications
{
    public static class ServiceCollectionExtensions
    {
        private const string ClientName = "specificationsClient";

        public static IServiceCollection AddSpecificationsInterServiceClient(this IServiceCollection builder, IConfiguration config,
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

            builder.AddHttpClient(HttpClientKeys.Specifications,
                    c =>
                    {
                        ApiOptions apiOptions = new ApiOptions();

                        config.Bind(ClientName, apiOptions);

                        ApiClientConfigurationOptions.SetDefaultApiClientConfigurationOptions(c, apiOptions, builder, ClientName);
                    })
                .ConfigurePrimaryHttpMessageHandler(() => new ApiClientHandler())
                .AddTransientHttpErrorPolicy(c => c.WaitAndRetryAsync(retryTimeSpans))
                .AddTransientHttpErrorPolicy(c => c.CircuitBreakerAsync(numberOfExceptionsBeforeCircuitBreaker, circuitBreakerFailurePeriod));

            builder
                .AddSingleton<ISpecificationsApiClient, SpecificationsApiClient>();

            return builder;
        }
    }
}
