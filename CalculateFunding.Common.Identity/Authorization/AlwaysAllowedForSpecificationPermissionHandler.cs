using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class AlwaysAllowedForSpecificationPermissionHandler : AuthorizationHandler<SpecificationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecificationRequirement requirement)
        {
            // Always grant permission
            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
