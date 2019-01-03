namespace CalculateFunding.Common.WebApi.Middleware
{
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;

	public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiKeyMiddlewareSettings(this IServiceCollection builder, IConfigurationRoot config)
        {
            ApiKeyMiddlewareOptions apiKeyMiddlewareOptions = new ApiKeyMiddlewareOptions();

            config.Bind("apiKeyMiddleware", apiKeyMiddlewareOptions);

            builder.AddSingleton<ApiKeyMiddlewareOptions>(apiKeyMiddlewareOptions);

            return builder;
        }
    }
}
