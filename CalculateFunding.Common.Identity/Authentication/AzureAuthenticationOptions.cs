using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace CalculateFunding.Common.Identity.Authentication
{
    public class AzureAuthenticationOptions : AuthenticationSchemeOptions
    {
        public PathString AuthMeEndpoint { get; set; } = "/.auth/me";

        public PathString AuthRefreshEndpoint { get; set; } = "/.auth/refresh";
    }
}
