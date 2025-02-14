﻿using CalculateFunding.Common.ApiClient.Models;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.Interfaces;
using Serilog;
using System.Collections.Generic;
using CalculateFunding.Common.ApiClient.FDS.Models;

namespace CalculateFunding.Common.ApiClient.FDS
{
    public class FDSApiClient : BaseApiClient, IFDSApiClient
    {
        private string FDSPrefix = "_FDS";
        private string FundingDataApiPrefix = "api/FundingData";
        public FDSApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null, string clientKey = null) 
            : base(httpClientFactory, clientKey ?? HttpClientKeys.FDS, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<IEnumerable<DatasetDefinitionByFundingStream>>> GetFDSDataSchema(string fundingStream, string fundingPeriod)
        {
            fundingStream = AdultStream.IsExists(fundingStream) ? AdultStream.GetParent() : fundingStream;

            return await PostAsync<IEnumerable<DatasetDefinitionByFundingStream>, DataSchemaRequest>($"{FundingDataApiPrefix}/schema/versions/query", new DataSchemaRequest()
            {
                FundingPeriodCode = fundingPeriod,
                FundingStreamCode = fundingStream
            });
        }
        public async Task<ApiResponse<IEnumerable<RemovedFieldDefinition>>> GetAllVersionsofSchema(string fundingStream, string fundingPeriod, string schemaName)
        {
            fundingStream = AdultStream.IsExists(fundingStream) ? AdultStream.GetParent() : fundingStream;

            return await PostAsync<IEnumerable<RemovedFieldDefinition>, DataSchemaRequest>($"{FundingDataApiPrefix}/schema/all/query", new DataSchemaRequest()
            {
                FundingPeriodCode = fundingPeriod,
                FundingStreamCode = fundingStream,
                SchemaName = schemaName
            });
        }

        public async Task<ApiResponse<FDSDatasetDefinition>> GetDatasetDefinition(string definitionId)
        {
            ApiResponse<FDSDatasetDefinition> fdsDefinition = await GetAsync<FDSDatasetDefinition>($"{FundingDataApiPrefix}/schema/" + definitionId);
            if (fdsDefinition == null || fdsDefinition.Content == null)
            {
                return fdsDefinition;
            }
            if (fdsDefinition.Content.Name != null)
            {
                fdsDefinition.Content.Name = FDSPrefix + fdsDefinition.Content.Id + fdsDefinition.Content.Name;
                fdsDefinition.Content.FDSTableDefinitions[0].Name = FDSPrefix + fdsDefinition.Content.Id + fdsDefinition.Content.Name;
            }
            return fdsDefinition;
        }

        public async Task<ApiResponse<IEnumerable<FDSDatasetVersion>>> GetDatasetVersionsByDefinitionId(string definitionId)
        {
            return await GetAsync<IEnumerable<FDSDatasetVersion>>($"{FundingDataApiPrefix}/FundingDataVersions/Schema/" + definitionId);
        }

        public async Task<ApiResponse<FundingDataVersionCount>> GetDatasetVersionsCountByDefinitionId(string definitionId)
        {
            return await GetAsync<FundingDataVersionCount>($"{FundingDataApiPrefix}/FundingDataVersions/Schema/count/" + definitionId);
        }

        public async Task<ApiResponse<FDSDatasetVersion>> GetDatasetVersionsBySnapshotId(string snapshotId)
        {
            return await GetAsync<FDSDatasetVersion>($"{FundingDataApiPrefix}/FundingDataVersions/" + snapshotId);
        }

        public async Task<ApiResponse<FDSDatasourceDataModel>> GetDatasourceDataBySnapshotId(string snapshotId)
        {
            return await GetAsync<FDSDatasourceDataModel>($"{FundingDataApiPrefix}/" + snapshotId);
        }
    }
}
