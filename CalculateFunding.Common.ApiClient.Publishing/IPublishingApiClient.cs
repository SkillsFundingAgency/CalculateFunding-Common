using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Publishing.Models;

namespace CalculateFunding.Common.ApiClient.Publishing
{
    public interface IPublishingApiClient
    {
        Task<ApiResponse<PublishedProviderVersion>> GetPublishedProviderVersion(string fundingStreamId, string fundingPeriodId, string providerId, string version);

        Task<ApiResponse<string>> GetPublishedProviderVersionBody(string publishedProviderVersionId);

        Task<ApiResponse<SpecificationCheckChooseForFundingResult>> CanChooseForFunding(string specificationId);
    }
}
