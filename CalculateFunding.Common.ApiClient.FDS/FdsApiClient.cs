using CalculateFunding.Common.ApiClient.Models;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.Interfaces;
using Serilog;
using System.Threading;
using System;
using System.Collections.Generic;
using CalculateFunding.Common.ApiClient.FDS.Models;

namespace CalculateFunding.Common.ApiClient.FDS
{
    public class FDSApiClient : BaseApiClient, IFDSApiClient
    {
        public FDSApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null) 
            : base(httpClientFactory, HttpClientKeys.FDS, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<IEnumerable<DatasetDefinitionByFundingStream>>> GetDatasetForFundingStream(string fundingStream)
        {
            return await GetAsync<IEnumerable<DatasetDefinitionByFundingStream>>($"FundingData/schemas/" + fundingStream);
        }

        public async Task<ApiResponse<FDSDatasetDefinition>> GetDatasetDefinition(string definitionId)
        {
            return await GetAsync<FDSDatasetDefinition>($"FundingData/schema/" + definitionId);
        }
        public async Task<ApiResponse<IEnumerable<FDSDatasetVersion>>> GetDatasetVersionsByDefinitionId(string definitionId)
        {
            return await GetAsync<IEnumerable<FDSDatasetVersion>>($"FundingData/FundingDataVersions/Schema/" + definitionId);
        }

        public async Task<ApiResponse<FundingDataVersionCount>> GetDatasetVersionsCountByDefinitionId(string definitionId)
        {
            return await GetAsync<FundingDataVersionCount>($"FundingData/FundingDataVersions/Schema/count/" + definitionId);
        }
    }
}
