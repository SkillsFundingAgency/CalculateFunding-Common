using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Publishing.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Publishing
{
    public class PublishingApiClient : BaseApiClient, IPublishingApiClient
    {
        public PublishingApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
        : base(httpClientFactory, HttpClientKeys.Publishing, logger, cancellationTokenProvider)
        { }

        public async Task<ApiResponse<PublishedProviderVersion>> GetPublishedProviderVersion(string fundingStreamId, string fundingPeriodId, string providerId, string version)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));
            Guard.IsNullOrWhiteSpace(version, nameof(version));

            string url = $"publishedproviderversions/{fundingStreamId}/{fundingPeriodId}/{providerId}/{version}";

            return await GetAsync<PublishedProviderVersion>(url);
        }

        public async Task<ApiResponse<string>> GetPublishedProviderVersionBody(string publishedProviderVersionId)
        {
            Guard.IsNullOrWhiteSpace(publishedProviderVersionId, nameof(publishedProviderVersionId));

            string url = $"publishedproviderversion/{publishedProviderVersionId}/body";

            return await GetAsync<string>(url);
        }

        public async Task<ApiResponse<SpecificationCheckChooseForFundingResult>> CanChooseForFunding(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"specifications/{specificationId}/funding/canChoose";

            return await GetAsync<SpecificationCheckChooseForFundingResult>(url);
        }
    }
}
