using System.Collections.Generic;
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
        private const string ProvidersBaseUri = "providers";
        private const string FundingBaseUri = "funding";
        private const string FundingStreamsBaseUri = "funding-streams";

        public ExternalApiClient(
            IHttpClientFactory httpClientFactory,
            string clientKey,
            ILogger logger,
            IBearerTokenProvider bearerTokenProvider,
            ICancellationTokenProvider cancellationTokenProvider = null) 
            : base(httpClientFactory, clientKey, logger, bearerTokenProvider, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<object>>GetProviderFundingVersion(string providerFundingVersion)
        {
            Guard.IsNullOrWhiteSpace(providerFundingVersion, nameof(providerFundingVersion));

            return await GetAsync<object>($"{Version}/{FundingBaseUri}/provider/{providerFundingVersion}");
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
            VariationReason[] variationReasons = null,
            int? pageSize = null,
            int? pageRef = null)
        {
            string uri = $"{Version}/{FundingBaseUri}/notifications";
            
            uri = pageRef.HasValue ? $"{uri}/{pageRef}" : uri;

            string queryString = AddQueryStringParametersIfSupplied("", nameof(fundingStreamIds), fundingStreamIds);
            queryString = AddQueryStringParametersIfSupplied(queryString, nameof(fundingPeriodIds), fundingPeriodIds);
            queryString = AddQueryStringParametersIfSupplied(queryString, nameof(groupingReasons), groupingReasons);
            queryString = AddQueryStringParametersIfSupplied(queryString, nameof(variationReasons), variationReasons);

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
            return await GetAsync<IEnumerable<FundingStream>>($"{Version}/{FundingStreamsBaseUri}");
        }

        public async Task<ApiResponse<IEnumerable<FundingPeriod>>> GetFundingPeriods(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<FundingPeriod>>(
                $"{Version}/{FundingStreamsBaseUri}/{fundingStreamId}/funding-periods");
        }

        public async Task<ApiResponse<string>> GetFundingTemplateSourceFile(string fundingStreamId, string fundingPeriodId, string majorVersion, string minorVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(majorVersion, nameof(majorVersion));
            Guard.IsNullOrWhiteSpace(minorVersion, nameof(minorVersion));

            return await GetAsync<string>(
                $"{Version}/{FundingStreamsBaseUri}/{fundingStreamId}/funding-periods/{fundingPeriodId}/templates/{majorVersion}.{minorVersion}");
        }

        public async Task<ApiResponse<IEnumerable<PublishedFundingTemplate>>> GetPublishedFundingTemplates(string fundingStreamId, string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
           

            return await GetAsync<IEnumerable<PublishedFundingTemplate>>(
                $"{Version}/{FundingStreamsBaseUri}/{fundingStreamId}/funding-periods/{fundingPeriodId}/templates");
        }

        public async Task<ApiResponse<ProviderVersionSearchResult>> GetPublishedProviderInformation(string publishedProviderVersion)
        {
            Guard.IsNullOrWhiteSpace(publishedProviderVersion, nameof(publishedProviderVersion));

            return await GetAsync<ProviderVersionSearchResult>($"{Version}/{ProvidersBaseUri}/{publishedProviderVersion}");
        }
    }
}
