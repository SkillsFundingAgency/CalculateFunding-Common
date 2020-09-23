using CalculateFunding.Common.Models.HealthCheck;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.WebApi.Middleware
{
    public class AuthenticatedHealthCheckMiddleware : HealthCheckMiddleware
    {
        public AuthenticatedHealthCheckMiddleware(IEnumerable<IHealthChecker> healthCheckers) : base(healthCheckers)
        {
        }

        public new async Task InvokeAsync(HttpContext context,
            RequestDelegate next)
        {
            if (context.Request.Path == "/healthcheck")
            {
                if ((context.User?.Identity?.IsAuthenticated).GetValueOrDefault() == false)
                {
                    context.Response.StatusCode = 401;
                }
                else
                {
                    await base.InvokeAsync(context, next);
                }
            }
            else
                await next(context);
        }
    }
}
