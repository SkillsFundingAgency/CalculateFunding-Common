namespace CalculateFunding.Common.WebApi.Middleware
{
	using System.Security.Claims;
	using System.Threading.Tasks;
    using CalculateFunding.Common.Models;
    using CalculateFunding.Common.Utility;
    using Microsoft.AspNetCore.Http;

	public class LoggedInUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserProfileProvider _userProfileProvider;

        public LoggedInUserMiddleware(RequestDelegate next, IUserProfileProvider userProfileProvider)
        {
            Guard.ArgumentNotNull(next, nameof(next));
            Guard.ArgumentNotNull(userProfileProvider, nameof(userProfileProvider));

            _next = next;
            _userProfileProvider = userProfileProvider;
            
        }

        public async Task Invoke(HttpContext context)
        {
            string userId = "unknown";
            string username = "unknown";

            if (context.Request.HttpContext.Request.Headers.ContainsKey("sfa-userid"))
                userId = context.Request.HttpContext.Request.Headers["sfa-userid"];

            if (context.Request.HttpContext.Request.Headers.ContainsKey("sfa-username"))
                username = context.Request.HttpContext.Request.Headers["sfa-username"];

            context.Request.HttpContext.User = new ClaimsPrincipal(new[]
            {
                new ClaimsIdentity(new []{ new Claim(ClaimTypes.Sid, userId), new Claim(ClaimTypes.Name, username) })
            });

            _userProfileProvider.UserProfile = new UserProfile(userId, username);

            // Call the next delegate/middleware in the pipeline
            await this._next(context);
        }
    }
}
