namespace CalculateFunding.Common.WebApi.Extensions
{
	using Microsoft.Extensions.DependencyInjection;
	using Middleware;

	public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHealthCheckMiddleware(this IServiceCollection services)
        {
            services.AddTransient<HealthCheckMiddleware>();

            return services;
        }

        public static IServiceCollection AddAuthenticatedHealthCheckMiddleware(this IServiceCollection services)
        {
            services.AddTransient<AuthenticatedHealthCheckMiddleware>();

            return services;
        }
    }
}
