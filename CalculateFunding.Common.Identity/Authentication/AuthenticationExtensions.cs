using System;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CalculateFunding.Common.Identity.Authentication
{
    public static class AuthenticationExtensions
    {
        /// <summary>
        /// Adds the <see cref="AzureAuthenticationHandler"/> for authentication.
        /// </summary>
        /// <param name="builder"><inheritdoc/></param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddAzureAuthentication(this AuthenticationBuilder builder)
            => builder.AddAzureAuthentication(AzureAuthenticationDefaults.AuthenticationScheme);

        /// <summary>
        /// Adds the <see cref="AzureAuthenticationHandler"/> for authentication.
        /// </summary>
        /// <param name="builder"><inheritdoc/></param>
        /// <param name="authenticationScheme">The schema for the Easy Auth handler.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddAzureAuthentication(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddAzureAuthentication(authenticationScheme, configureOptions: null);

        /// <summary>
        /// Adds the <see cref="AzureAuthenticationHandler"/> for authentication.
        /// </summary>
        /// <param name="builder"><inheritdoc/></param>
        /// <param name="configureOptions">A callback to configure <see cref="EasyAuthAuthenticationOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddAzureAuthentication(this AuthenticationBuilder builder, Action<AzureAuthenticationOptions> configureOptions)
            => builder.AddAzureAuthentication(AzureAuthenticationDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Adds the <see cref="AzureAuthenticationHandler"/> for authentication.
        /// </summary>
        /// <param name="builder"><inheritdoc/></param>
        /// <param name="authenticationScheme">The schema for the Easy Auth handler.</param>
        /// <param name="configureOptions">A callback to configure <see cref="EasyAuthAuthenticationOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddAzureAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<AzureAuthenticationOptions> configureOptions)
            => builder.AddAzureAuthentication(authenticationScheme, displayName: AzureAuthenticationDefaults.DisplayName, configureOptions: configureOptions);

        /// <summary>
        /// Adds the <see cref="AzureAuthenticationHandler"/> for authentication.
        /// </summary>
        /// <param name="builder"><inheritdoc/></param>
        /// <param name="authenticationScheme">The schema for the Easy Auth handler.</param>
        /// <param name="displayName">The display name for the Easy Auth handler.</param>
        /// <param name="configureOptions">A callback to configure <see cref="EasyAuthAuthenticationOptions"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static AuthenticationBuilder AddAzureAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<AzureAuthenticationOptions> configureOptions)
            => builder.AddScheme<AzureAuthenticationOptions, AzureAuthenticationHandler>(authenticationScheme, displayName, configureOptions);

        public static IServiceCollection AddAzureAuthenticationHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient(AzureAuthenticationHandler.AzureAuthenticationHttpClientName);

            services.AddHttpClient(AzureAuthenticationHandler.GraphHttpClientName);

            return services;
        }
    }
}
