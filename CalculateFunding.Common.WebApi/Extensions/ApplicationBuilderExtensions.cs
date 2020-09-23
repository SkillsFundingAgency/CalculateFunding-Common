namespace CalculateFunding.Common.WebApi.Extensions
{
	using Microsoft.AspNetCore.Builder;
	using Middleware;

	public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHealthCheckMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<HealthCheckMiddleware>();
        }

        public static IApplicationBuilder UseAuthenticatedHealthCheckMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticatedHealthCheckMiddleware>();
        }
    }
}
