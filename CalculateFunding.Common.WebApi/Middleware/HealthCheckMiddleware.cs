﻿namespace CalculateFunding.Common.WebApi.Middleware
{
	using System;
	using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
	using Common.Extensions;
	using Microsoft.AspNetCore.Http;
	using Models.HealthCheck;

	public class HealthCheckMiddleware : IMiddleware
    {
        private IEnumerable<IHealthChecker> _healthCheckers;

        public HealthCheckMiddleware(IEnumerable<IHealthChecker> healthCheckers)
        {
            _healthCheckers = healthCheckers;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if(context.Request.Path == "/healthcheck")
            {
                OverallHealth overallHealth = new OverallHealth { 
                    OverallHealthOk = true,
                    BuildNumber = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion
                };

                if (_healthCheckers != null)
                {
                    foreach (var item in _healthCheckers)
                    {
                        try
                        {
                            ServiceHealth health = await item.IsHealthOk();
                            overallHealth.OverallHealthOk = overallHealth.OverallHealthOk && health.Dependencies.Min(x => x.HealthOk);
                            overallHealth.Services.Add(health);
                        }catch(Exception ex)
                        {
                            ServiceHealth failure = new ServiceHealth { Name = item.GetType().GetFriendlyName() };
                            failure.Dependencies.Add(new DependencyHealth { DependencyName = "IoC Failure", HealthOk = false, Message = ex.Message });
                            overallHealth.OverallHealthOk = false;
                            overallHealth.Services.Add(failure);
                        }
                    }
                }

                string overallHealthJson = Newtonsoft.Json.JsonConvert.SerializeObject(overallHealth);
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(overallHealthJson);
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}
