using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
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
            string groupId = Guid.NewGuid().ToString();
            string authprovider = "EasyAuthProvider";
            string principal = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{{'auth_typ':'{authprovider}','claims':[{{'typ':'{ClaimTypes.Name}','val':'test@testdomain.com'}}, {{'typ':'{Constants.GroupsClaimType}','val':'{groupId}'}}, {{'typ':'{ClaimTypes.GivenName}','val':'Fred'}}],'name_typ':'','role_typ':''}}"));
            AzureAuthenticationHandler handler = await CreateAzureAuthenticationHandler(principal, authprovider);
            
            // Act
            AuthenticateResult result = await handler.AuthenticateAsync();

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Principal.HasClaim(c => c.Type == "AuthenticationProvider" && c.Value == "EasyAuthProvider");
            result.Ticket.Principal.Identity.Name.Should().Be("test@testdomain.com");
            result.Ticket.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName && c.Value == "Fred");
            result.Ticket.Principal.HasClaim(c => c.Type == Constants.GroupsClaimType && c.Value == groupId);
        }

        private static async Task<AzureAuthenticationHandler> CreateAzureAuthenticationHandler(string principal, string authProvider)
        {
            AzureAuthenticationOptions options = new AzureAuthenticationOptions();
            IOptionsMonitor<AzureAuthenticationOptions> optionsMonitor = Substitute.For<IOptionsMonitor<AzureAuthenticationOptions>>();
            optionsMonitor.CurrentValue.Returns(options);

            ILoggerFactory logger = Substitute.For<ILoggerFactory>();
            UrlEncoder encoder = Substitute.For<UrlEncoder>();
            ISystemClock clock = new SystemClock();

            HeaderDictionary headers = new HeaderDictionary
            {
                { "X-MS-CLIENT-PRINCIPAL-IDP", authProvider },
                { "X-MS-CLIENT-PRINCIPAL", principal}
            };

            HttpContext context = Substitute.For<HttpContext>();
            context.Request.Headers.Returns(headers);
            context.Request.Scheme.Returns("https");
            context.Request.Host.Returns(new HostString("test.com"));

            AuthenticationScheme scheme = new AuthenticationScheme(AzureAuthenticationDefaults.AuthenticationScheme, AzureAuthenticationDefaults.DisplayName, typeof(AzureAuthenticationHandler));

            AzureAuthenticationHandler handler = new AzureAuthenticationHandler(optionsMonitor, logger, encoder, clock);
            await handler.InitializeAsync(scheme, context);
            return handler;
        }
    }
}
