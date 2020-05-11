﻿using System;
using System.Threading.Tasks;
using CalculateFunding.Common.Caching;
using CalculateFunding.Common.Utility;

namespace CalculateFunding.Common.ApiClient.Bearer
{
    public class AzureBearerTokenProvider : IBearerTokenProvider
    {
        private readonly ICacheProvider _cacheProvider;
        private readonly AzureBearerTokenOptions _azureBearerTokenOptions;
        private readonly IAzureBearerTokenProxy _azureBearerTokenProxy;

        public AzureBearerTokenProvider(IAzureBearerTokenProxy azureBearerTokenProxy, ICacheProvider cacheProvider, AzureBearerTokenOptions azureBearerTokenOptions)
        {
            Guard.ArgumentNotNull(azureBearerTokenProxy, nameof(azureBearerTokenProxy));
            Guard.ArgumentNotNull(cacheProvider, nameof(cacheProvider));
            Guard.ArgumentNotNull(azureBearerTokenOptions, nameof(azureBearerTokenOptions));

            _cacheProvider = cacheProvider;
            _azureBearerTokenOptions = azureBearerTokenOptions;
            _azureBearerTokenProxy = azureBearerTokenProxy;
        }

        public async Task<string> GetToken()
        {
            string accessToken = await _cacheProvider.GetAsync<string>(_azureBearerTokenOptions.ClientId);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                return accessToken;
            }

            AzureBearerToken token = await _azureBearerTokenProxy.FetchToken(_azureBearerTokenOptions);

            if (token == null)
            {
                throw new Exception($"Failed to refersh access token for url: {_azureBearerTokenOptions.Url}");
            }

            double cacheExpiryLength = (0.9 * token.ExpiryLength);

            await _cacheProvider.SetAsync<string>(_azureBearerTokenOptions.ClientId, token.AccessToken, TimeSpan.FromSeconds(cacheExpiryLength), false, null);

            return token.AccessToken;
        }

        public async Task<(bool Ok, string Message)> IsHealthOk()
        {
            return await _cacheProvider.IsHealthOk();
        }
    }
}
