using CalculateFunding.Common.Identity.Authorization.Models;
using Microsoft.AspNetCore.Authorization;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class FundingStreamRequirement : IAuthorizationRequirement
    {
        public FundingStreamActionTypes ActionType { get; set; }

        public FundingStreamRequirement(FundingStreamActionTypes fundingStreamAction)
        {
            ActionType = fundingStreamAction;
        }
    }
}
