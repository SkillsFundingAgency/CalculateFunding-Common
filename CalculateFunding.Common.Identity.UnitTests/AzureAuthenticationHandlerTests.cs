using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CalculateFunding.Common.Identity.Authentication;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace CalculateFunding.Common.Identity.UnitTests
{
    [TestClass]
    public class AzureAuthenticationHandlerTests
    {
        [TestMethod]
        public async Task AddAzureAuthentication_CorrectlyAddsAzureAuthenticationHandler()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddAuthentication().AddAzureAuthentication();
            ServiceProvider sp = services.BuildServiceProvider();
            IAuthenticationSchemeProvider schemeProvider = sp.GetRequiredService<IAuthenticationSchemeProvider>();

            // Act
            AuthenticationScheme scheme = await schemeProvider.GetSchemeAsync(AzureAuthenticationDefaults.AuthenticationScheme);

            // Assert
            scheme.Should().NotBeNull();
            scheme.HandlerType.Name.Should().Be(nameof(AzureAuthenticationHandler));
            scheme.DisplayName.Should().Be(AzureAuthenticationDefaults.DisplayName);
            scheme.Name.Should().Be(AzureAuthenticationDefaults.AuthenticationScheme);
        }

        [TestMethod]
        public async Task AuthenticateAsync_WhenUserKnown_ThenSuccessfulAuthentication()
        {
            // Arrange
            string authMeResponse = "[{'access_token': '', 'user_id': 'test@testdomain.com', 'user_claims': [{'typ': 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname', 'val': 'Fred' }]}]";
            string graphResponse = "{'@odata.context': 'https://graph.microsoft.com/v1.0/$metadata#groups(id,displayName,securityEnabled)','value': [{'id': 'e73a963b-059e-45e4-acb1-f3fe748f93a4','displayName': 'TestGroup','securityEnabled': true}]}";
            AzureAuthenticationHandler handler = await CreateAzureAuthenticationHandler(authMeResponse, graphResponse, DateTime.Now.AddMinutes(30));

            // Act
            AuthenticateResult result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Ticket.Principal.Identity.Name.Should().Be("test@testdomain.com");
            result.Ticket.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName && c.Value == "Fred");
            result.Ticket.Principal.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "e73a963b-059e-45e4-acb1-f3fe748f93a4");
        }

        [TestMethod]
        public async Task AuthenticateAsync_WhenAccessTokenExpired_ThenAccessTokenRefreshed()
        {
            // Arrange
            string authMeResponse = "[{'access_token': '', 'user_id': 'test@testdomain.com', 'user_claims': [{'typ': 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname', 'val': 'Fred' }]}]";
            string graphResponse = "{'@odata.context': 'https://graph.microsoft.com/v1.0/$metadata#groups(id,displayName,securityEnabled)','value': [{'id': 'e73a963b-059e-45e4-acb1-f3fe748f93a4','displayName': 'TestGroup','securityEnabled': true}]}";

            MockHttpMessageHandler authMessageHandler = new MockHttpMessageHandler();
            authMessageHandler
                .Expect(HttpMethod.Get, "https://test.com/.auth/refresh")
                .Respond(HttpStatusCode.OK);

            authMessageHandler
                .Expect(HttpMethod.Get, "https://test.com/.auth/me")
                .Respond("application/json", authMeResponse);

            MockHttpMessageHandler graphMessageHandler = CreateStandardGraphMessageHandler(graphResponse);

            AzureAuthenticationHandler handler = await CreateAzureAuthenticationHandler(authMessageHandler, graphMessageHandler, DateTime.Now.AddMinutes(-5));

            // Act
            AuthenticateResult result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeTrue();
            authMessageHandler.VerifyNoOutstandingExpectation();
        }

        [TestMethod]
        public async Task AuthenticateAsync_WhenAccessTokenExpired_AndRefreshFails_ThenAuthenticationFails()
        {
            // Arrange
            MockHttpMessageHandler authMessageHandler = new MockHttpMessageHandler();
            authMessageHandler
                .Expect(HttpMethod.Get, "https://test.com/.auth/refresh")
                .Respond(HttpStatusCode.BadRequest);

            var authMeRequest = authMessageHandler
                .When(HttpMethod.Get, "https://test.com/.auth/me")
                .Respond(HttpStatusCode.OK);

            MockHttpMessageHandler graphMessageHandler = CreateStandardGraphMessageHandler("");

            AzureAuthenticationHandler handler = await CreateAzureAuthenticationHandler(authMessageHandler, graphMessageHandler, DateTime.Now.AddMinutes(-5));

            // Act
            AuthenticateResult result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
            authMessageHandler.GetMatchCount(authMeRequest).Should().Be(0, "Auth Me call should not be called");
        }

        [TestMethod]
        public async Task AuthenticateAsync_WhenAuthMeCallFails_ThenAuthenticationFails()
        {
            // Arrange
            MockHttpMessageHandler authMessageHandler = new MockHttpMessageHandler();
            authMessageHandler
                .When(HttpMethod.Get, "https://test.com/.auth/refresh")
                .Respond(HttpStatusCode.OK);

            authMessageHandler
                .When(HttpMethod.Get, "https://test.com/.auth/me")
                .Respond(HttpStatusCode.BadRequest);

            MockHttpMessageHandler graphMessageHandler = CreateStandardGraphMessageHandler("");

            AzureAuthenticationHandler handler = await CreateAzureAuthenticationHandler(authMessageHandler, graphMessageHandler, DateTime.Now.AddMinutes(5));

            // Act
            AuthenticateResult result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task AuthenticateAsync_WhenGraphCallFails_ThenAuthenticationFails()
        {
            // Arrange
            string authMeResponse = "[{'access_token': '', 'user_id': 'test@testdomain.com', 'user_claims': [{'typ': 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname', 'val': 'Fred' }]}]";
            MockHttpMessageHandler authMessageHandler = CreateStandardAuthMessageHandler(authMeResponse);

            MockHttpMessageHandler graphMessageHandler = new MockHttpMessageHandler();
            graphMessageHandler
                .When("https://graph.microsoft.com/v1.0/me/memberOf/$/microsoft.graph.group?$select=id,displayName,securityEnabled")
                .Respond(HttpStatusCode.BadRequest);

            AzureAuthenticationHandler handler = await CreateAzureAuthenticationHandler(authMessageHandler, graphMessageHandler, DateTime.Now.AddMinutes(5));

            // Act
            AuthenticateResult result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task AuthenticateAsync_WhenAuthMeJsonInvalid_ThenAuthenticationFails()
        {
            // Arrange
            string authMeResponse = "[{'access_token': '',";
            MockHttpMessageHandler authMessageHandler = CreateStandardAuthMessageHandler(authMeResponse);

            MockHttpMessageHandler graphMessageHandler = CreateStandardGraphMessageHandler("");

            AzureAuthenticationHandler handler = await CreateAzureAuthenticationHandler(authMessageHandler, graphMessageHandler, DateTime.Now.AddMinutes(5));

            // Act
            AuthenticateResult result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task AuthenticateAsync_WhenGraphCallReturnsInvalidJson_ThenAuthenticationFails()
        {
            // Arrange
            string authMeResponse = "[{'access_token': '', 'user_id': 'test@testdomain.com', 'user_claims': [{'typ': 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname', 'val': 'Fred' }]}]";
            MockHttpMessageHandler authMessageHandler = CreateStandardAuthMessageHandler(authMeResponse);

            string graphResponse = "{'@odata.context': ";
            MockHttpMessageHandler graphMessageHandler = CreateStandardGraphMessageHandler(graphResponse);

            AzureAuthenticationHandler handler = await CreateAzureAuthenticationHandler(authMessageHandler, graphMessageHandler, DateTime.Now.AddMinutes(5));

            // Act
            AuthenticateResult result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeFalse();
        }

        private static async Task<AzureAuthenticationHandler> CreateAzureAuthenticationHandler(string authMeResponse, string graphResponse, DateTime accessTokenExpiryTime)
        {
            MockHttpMessageHandler mockAuthHttpHandler = CreateStandardAuthMessageHandler(authMeResponse);

            MockHttpMessageHandler mockGraphHttpHandler = CreateStandardGraphMessageHandler(graphResponse);

            return await CreateAzureAuthenticationHandler(mockAuthHttpHandler, mockGraphHttpHandler, accessTokenExpiryTime);
        }

        private static MockHttpMessageHandler CreateStandardGraphMessageHandler(string graphResponse)
        {
            MockHttpMessageHandler mockGraphHttpHandler = new MockHttpMessageHandler();
            mockGraphHttpHandler
                .When("https://graph.microsoft.com/v1.0/me/memberOf/$/microsoft.graph.group?$select=id,displayName,securityEnabled")
                .Respond("application/json", graphResponse);
            return mockGraphHttpHandler;
        }

        private static MockHttpMessageHandler CreateStandardAuthMessageHandler(string authMeResponse)
        {
            MockHttpMessageHandler mockAuthHttpHandler = new MockHttpMessageHandler();
            mockAuthHttpHandler
                .When(HttpMethod.Get, "https://test.com/.auth/me")
                .Respond("application/json", authMeResponse);

            mockAuthHttpHandler
                .When(HttpMethod.Get, "https://test.com/.auth/refresh")
                .Respond(HttpStatusCode.OK);
            return mockAuthHttpHandler;
        }

        private static async Task<AzureAuthenticationHandler> CreateAzureAuthenticationHandler(MockHttpMessageHandler authMessageHandler, MockHttpMessageHandler graphMessageHandler, DateTime accessTokenExpiryTime)
        {
            AzureAuthenticationOptions options = new AzureAuthenticationOptions();
            IOptionsMonitor<AzureAuthenticationOptions> optionsMonitor = Substitute.For<IOptionsMonitor<AzureAuthenticationOptions>>();
            optionsMonitor.CurrentValue.Returns(options);

            ILoggerFactory logger = Substitute.For<ILoggerFactory>();
            UrlEncoder encoder = Substitute.For<UrlEncoder>();
            ISystemClock clock = new SystemClock();

            HttpClient authHttpClient = authMessageHandler.ToHttpClient();

            HttpClient graphHttpClient = graphMessageHandler.ToHttpClient();

            IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
            httpClientFactory
                .CreateClient(Arg.Is(AzureAuthenticationHandler.AzureAuthenticationHttpClientName))
                .Returns(authHttpClient);
            httpClientFactory
                .CreateClient(Arg.Is(AzureAuthenticationHandler.GraphHttpClientName))
                .Returns(graphHttpClient);

            HeaderDictionary headers = new HeaderDictionary
            {
                { "X-MS-TOKEN-AAD-EXPIRES-ON", accessTokenExpiryTime.ToString() }
            };

            HttpContext context = Substitute.For<HttpContext>();
            context.Request.Headers.Returns(headers);
            context.Request.Scheme.Returns("https");
            context.Request.Host.Returns(new HostString("test.com"));

            AuthenticationScheme scheme = new AuthenticationScheme(AzureAuthenticationDefaults.AuthenticationScheme, AzureAuthenticationDefaults.DisplayName, typeof(AzureAuthenticationHandler));

            AzureAuthenticationHandler handler = new AzureAuthenticationHandler(optionsMonitor, logger, encoder, clock, httpClientFactory);
            await handler.InitializeAsync(scheme, context);
            return handler;
        }
    }
}
