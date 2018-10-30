using CalculateFunding.Common.Identity.Authorization.Models;
using Microsoft.AspNetCore.Authorization;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class SpecificationRequirement : IAuthorizationRequirement
    {
        public SpecificationActionTypes ActionType { get;set; }

        public SpecificationRequirement(SpecificationActionTypes specificationAction)
        {
            ActionType = specificationAction;
        }
    }
}
