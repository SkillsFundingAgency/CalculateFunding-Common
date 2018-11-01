using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class AlwaysAllowedForFundingStreamPermissionHandler : AuthorizationHandler<FundingStreamRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FundingStreamRequirement requirement)
        {
            // Always grant permission
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
