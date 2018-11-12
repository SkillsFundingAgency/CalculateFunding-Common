using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CalculateFunding.Common.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace CalculateFunding.Common.Identity.Authentication
{
    public class AzureAuthenticationHandler : AuthenticationHandler<AzureAuthenticationOptions>
    {
        public const string AzureAuthenticationHttpClientName = "AzureAuthentication";
        public const string GraphHttpClientName = "AzureAuthenticationGraph";
        private const string AADAccessTokenExpiresOnHeaderName = "X-MS-TOKEN-AAD-EXPIRES-ON";

        private readonly HttpClient _httpClient;
        private readonly HttpClient _graphHttpClient;

        public AzureAuthenticationHandler(IOptionsMonitor<AzureAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IHttpClientFactory httpClientFactory)
            : base(options, logger, encoder, clock)
        {
            Guard.ArgumentNotNull(httpClientFactory, nameof(httpClientFactory));

            _httpClient = httpClientFactory.CreateClient(AzureAuthenticationHttpClientName);
            _graphHttpClient = httpClientFactory.CreateClient(GraphHttpClientName);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Logger.LogInformation($"Entering authentication process with {nameof(AzureAuthenticationHandler)}");

            if ((Context.User == null || Context.User.Identity == null || !Context.User.Identity.IsAuthenticated)
                && (Context.Request.Path != $"/{Options.AuthMeEndpoint}" || Context.Request.Path != $"/{Options.AuthRefreshEndpoint}"))
            {
                try
                {
                    AzureAuthenticationResponse authResponse = await GetUserDetailsFromAzureAuthentication();

                    await GetRoleMembershipsForCurrentUser(authResponse);

                    AuthenticationTicket ticket = BuildAuthenticationTicketFromAuthenticationResponse(authResponse);

                    Logger.LogInformation("Setting context user to authenticated principal.");
                    Context.User = ticket.Principal;

                    Logger.LogInformation($"Authentication successful, exiting {nameof(AzureAuthenticationHandler)}");
                    return AuthenticateResult.Success(ticket);
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

        private async Task<AzureAuthenticationResponse> GetUserDetailsFromAzureAuthentication()
        {
            string graphAccessTokenExpiresOnValue = Context.Request.Headers[AADAccessTokenExpiresOnHeaderName][0];

            DateTime graphAccessTokenExpiresOn = DateTime.Parse(graphAccessTokenExpiresOnValue);

            if (graphAccessTokenExpiresOn < DateTime.Now.AddMinutes(-1))
            {
                // Need to refresh the token
                HttpRequestMessage refreshRequest = CreateAuthRequest(Options.AuthRefreshEndpoint);

                try
                {
                    HttpResponseMessage refreshResponse = await ExecuteAuthRequest(refreshRequest);

                    if (!refreshResponse.IsSuccessStatusCode)
                    {
                        throw new WebException("Call to auth refresh endpoint was not successful");
                    }
                }
                catch (Exception ex)
                {
                    throw new WebException("Error calling auth refresh endpoint", ex);
                }
            }

            HttpRequestMessage authRequest = CreateAuthRequest(Options.AuthMeEndpoint);

            try
            {
                HttpResponseMessage authResponse = await ExecuteAuthRequest(authRequest);

                if (!authResponse.IsSuccessStatusCode)
                {
                    throw new WebException("Call to auth me endpoint was not successful");
                }

                string authResponseBody = await authResponse.Content.ReadAsStringAsync();

                JArray authResponseJson = JArray.Parse(authResponseBody);
                JObject authMeDetails = (JObject)authResponseJson[0];
                string graphAccessToken = authMeDetails.Value<string>("access_token");

                return new AzureAuthenticationResponse { AuthMeResponse = authMeDetails, GraphAccessToken = graphAccessToken };
            }
            catch
            {
                throw new WebException("Could not retreive json from auth me endpoint.");
            }
        }

        private async Task GetRoleMembershipsForCurrentUser(AzureAuthenticationResponse userDetails)
        {
            string graphUrl = "https://graph.microsoft.com/v1.0/me/memberOf/$/microsoft.graph.group?$select=id,displayName,securityEnabled";

            HttpRequestMessage graphRequest = new HttpRequestMessage(HttpMethod.Get, graphUrl);
            graphRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userDetails.GraphAccessToken);
            Logger.LogInformation("Making call to MS Graph to get group memberships");

            using (HttpResponseMessage graphResponse = await _graphHttpClient.SendAsync(graphRequest))
            {
                if (!graphResponse.IsSuccessStatusCode)
                {
                    Logger.LogError($"Unable to fetch users group memberships. {graphResponse.StatusCode}");
                    throw new WebException($"Unable to fetch users group memberships. {graphResponse.StatusCode}");
                }

                string content = await graphResponse.Content.ReadAsStringAsync();

                try
                {
                    Logger.LogInformation("Parsing group memberships");
                    JToken result = JToken.Parse(content);
                    userDetails.GroupMemberships = result.Value<JArray>("value");
                }
                catch
                {
                    Logger.LogError("Could not retreive json from graph endpoint.");
                    throw new WebException("Could not retreive json from graph endpoint.");
                }
            }
        }

        private AuthenticationTicket BuildAuthenticationTicketFromAuthenticationResponse(AzureAuthenticationResponse userDetails)
        {
            string userId = userDetails.AuthMeResponse["user_id"].Value<string>();

            Logger.LogDebug("Building authentication ticket for user : {0}", userId);

            GenericIdentity identity = new GenericIdentity(userId);

            Logger.LogInformation("Adding claims from payload");

            List<Claim> claims = new List<Claim>();

            foreach (JToken claim in userDetails.AuthMeResponse["user_claims"])
            {
                claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
            }

            Logger.LogInformation("Adding claims for groups");

            foreach (JToken group in userDetails.GroupMemberships)
            {
                bool isSecurityGroup = group["securityEnabled"].Value<bool>();
                if (isSecurityGroup)
                {
                    claims.Add(new Claim(ClaimTypes.Role, group["id"].ToString()));
                }
            }

            Logger.LogInformation("Add claims to new identity");
            identity.AddClaims(claims);

            GenericPrincipal p = new GenericPrincipal(identity, null);

            return new AuthenticationTicket(p, AzureAuthenticationDefaults.AuthenticationScheme);
        }

        private HttpRequestMessage CreateAuthRequest(string endPoint)
        {
            string uriString = $"{Context.Request.Scheme}://{Context.Request.Host}{endPoint}";
            Logger.LogInformation($"Creating request for {uriString}");
            List<string> cookieValues = new List<string>();

            // Have to copy the cookies into a header as it is not possible to add cookies directly when using a IHttpClientFactory
            foreach (KeyValuePair<string, string> c in Context.Request.Cookies)
            {
                cookieValues.Add($"{c.Key}={c.Value}");
            }

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uriString);
            Logger.LogInformation($"Adding cookies as headers to request {string.Join("; ", cookieValues)}");
            request.Headers.Add("Cookie", string.Join("; ", cookieValues));

            foreach (KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues> header in Context.Request.Headers)
            {
                if (header.Key.StartsWith("X-ZUMO-"))
                {
                    Logger.LogInformation($"Adding header to request {header.Key}/{header.Value[0]}");
                    request.Headers.Add(header.Key, header.Value[0]);
                }
            }

            return request;
        }

        private async Task<HttpResponseMessage> ExecuteAuthRequest(HttpRequestMessage request)
        {
            Logger.LogInformation($"Attempting to call auth endpoint '{request.RequestUri.ToString()}'");

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Logger.LogDebug($"Request to authentication endpoint ({request.RequestUri.ToString()}) was not sucessful. Status code: {response.StatusCode}");
            }

            return response;
        }
    }
}
