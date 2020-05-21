﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Bearer;
using CalculateFunding.Common.ApiClient.External.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.External
{
    public class ExternalApiClient : BearerBaseApiClient, IExternalApiClient
    {
        private const string Version = "v3";
        private const string FundingBaseUri = "funding";

        public ExternalApiClient(
            IHttpClientFactory httpClientFactory,
            string clientKey,
            ILogger logger,
            IBearerTokenProvider bearerTokenProvider,
            ICancellationTokenProvider cancellationTokenProvider = null) 
            : base(httpClientFactory, clientKey, logger, bearerTokenProvider, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<AtomFeed<object>>> GetProviderFundingVersion(string providerFundingVersion)
        {
            Guard.IsNullOrWhiteSpace(providerFundingVersion, nameof(providerFundingVersion));

            return await GetAsync<AtomFeed<object>>($"{Version}/{FundingBaseUri}/provider/{providerFundingVersion}");
        }

        public async Task<ApiResponse<IEnumerable<dynamic>>> GetFundings(string publishedProviderVersion)
        {
            Guard.IsNullOrWhiteSpace(publishedProviderVersion, nameof(publishedProviderVersion));

            return await GetAsync<IEnumerable<dynamic>>($"{Version}/{FundingBaseUri}/provider/{publishedProviderVersion}/fundings");
        }

        public async Task<ApiResponse<string>> GetFundingById(string id)
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));

            return await GetAsync<string>($"{Version}/{FundingBaseUri}/byId/{id}");
        }
        
        public async Task<ApiResponse<AtomFeed<object>>> GetFundingNotifications(string[] fundingStreamIds = null,
            string[] fundingPeriodIds = null,
            GroupingReason[] groupingReasons = null,
            int? pageSize = null,
            int? pageRef = null)
        {
            string uri = $"{Version}/{FundingBaseUri}/notifications";
            
            uri = pageRef.HasValue ? $"{uri}/{pageRef}" : uri;

            string queryString = AddQueryStringParametersIfSupplied("", nameof(fundingStreamIds), fundingStreamIds);
            queryString = AddQueryStringParametersIfSupplied(queryString, nameof(fundingPeriodIds), fundingPeriodIds);
            queryString = AddQueryStringParametersIfSupplied(queryString, nameof(groupingReasons), groupingReasons);

            if (pageSize.HasValue)
            {
                queryString = AddQueryStringParametersIfSupplied(queryString, nameof(pageSize), new[] {pageSize});
            }

            uri = string.IsNullOrWhiteSpace(queryString) ? uri : $"{uri}?{queryString}";
            
            return await GetAsync<AtomFeed<object>>(uri);
        }

        private string AddQueryStringParametersIfSupplied<T>(string queryString, string parameterName, T[] parameters)
        {
            if (parameters.IsNullOrEmpty())
            {
                return queryString;
            }

            string queryStringParameters = string.Join($"&{parameterName}=", parameters);
            string queryStringParameter = $"{parameterName}={queryStringParameters}";
            
            return queryString.Length == 0 ? queryStringParameter : $"{queryString}&{queryStringParameter}";
        }

        public async Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreams()
        {
            return await GetAsync<IEnumerable<FundingStream>>($"{Version}/funding-streams");
        }
    }
}
