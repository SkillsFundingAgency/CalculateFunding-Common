using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CalculateFunding.Common.Identity.Authentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CalculateFunding.Common.Identity.Authentication
{
    public class AzureAuthenticationHandler : AuthenticationHandler<AzureAuthenticationOptions>
    {
        private const string EasyAuthProviderHeaderName = "X-MS-CLIENT-PRINCIPAL-IDP";
        private const string PrincipalHeaderName = "X-MS-CLIENT-PRINCIPAL";

        public AzureAuthenticationHandler(IOptionsMonitor<AzureAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Logger.LogInformation($"Entering authentication process with {nameof(AzureAuthenticationHandler)}");

            if ((Context.User == null || Context.User.Identity == null || !Context.User.Identity.IsAuthenticated)
                && (Context.Request.Path != $"/{Options.AuthMeEndpoint}" || Context.Request.Path != $"/{Options.AuthRefreshEndpoint}"))
            {
                try
                {
                    string easyAuthProvider = Context.Request.Headers[EasyAuthProviderHeaderName].FirstOrDefault();
                    string msClientPrincipalEncoded = Context.Request.Headers[PrincipalHeaderName].FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(msClientPrincipalEncoded))
                    {
                        return AuthenticateResult.NoResult();
                    }

                    byte[] decodedBytes = Convert.FromBase64String(msClientPrincipalEncoded);
                    string msClientPrincipalDecoded = System.Text.Encoding.Default.GetString(decodedBytes);
                    MsClientPrincipal clientPrincipal = JsonConvert.DeserializeObject<MsClientPrincipal>(msClientPrincipalDecoded);
                    ClaimsPrincipal principal = new ClaimsPrincipal();
                    List<Claim> claims = new List<Claim>(clientPrincipal.Claims.Select(x => new Claim(x.Type, x.Value)));

                    foreach(Claim claim in claims)
                    {
                        Logger.LogInformation($"adding claim Type: {claim.Type} Value: {claim.Value}");
                    }

                    principal.AddIdentity(new ClaimsIdentity(claims, clientPrincipal.AuthenticationType, clientPrincipal.NameType, clientPrincipal.RoleType));
                    AuthenticationTicket ticket = new AuthenticationTicket(principal, easyAuthProvider);
                    Context.User = ticket.Principal;

                    AuthenticateResult success = AuthenticateResult.Success(ticket);

                    return await Task.FromResult(success);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Failed to process identity in {nameof(AzureAuthenticationHandler)}");
                    return AuthenticateResult.Fail(ex.Message);
                }
            }
            else
            {
                Logger.LogInformation($"Identity already set, nothing to process in {nameof(AzureAuthenticationHandler)}");
                return AuthenticateResult.NoResult();
            }
        }
    }
}
