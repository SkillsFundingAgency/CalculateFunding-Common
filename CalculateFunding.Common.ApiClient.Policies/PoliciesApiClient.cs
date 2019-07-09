using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Policies.Models.ViewModels;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Policies
{
    public class PoliciesApiClient : BaseApiClient, IPoliciesApiClient
    {
        public PoliciesApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
         : base(httpClientFactory, HttpClientKeys.Policies, logger, cancellationTokenProvider)
        { }

        public async Task<ApiResponse<FundingConfiguration>> GetFundingConfiguration(string fundingStreamId, string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));  

            string url = $"configuration/{fundingStreamId}/{fundingPeriodId}";

            return await GetAsync<FundingConfiguration>(url);
        }

        public async Task<ApiResponse<Period>> GetFundingPeriodById(string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            string url = $"fundingperiods/{fundingPeriodId}";

            return await GetAsync<Period>(url);
        }

        public async Task<ApiResponse<IEnumerable<Period>>> GetFundingPeriods()
        {
            string url = "fundingperiods";

            return await GetAsync<IEnumerable<Period>>(url);
        }

        public async Task<ApiResponse<string>> GetFundingSchemaByVersion(string schemaVersion)
        {
            Guard.IsNullOrWhiteSpace(schemaVersion, nameof(schemaVersion));

            string url = $"schemas/{schemaVersion}";

            return await GetAsync<string>(url);
        }

        public async Task<ApiResponse<FundingStream>> GetFundingStreamById(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            string url = $"fundingstreams/{fundingStreamId}";

            return await GetAsync<FundingStream>(url);
        }

        public async Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreams()
        {
            string url = "fundingstreams";

            return await GetAsync<IEnumerable<FundingStream>>(url);
        }

        public async Task<ApiResponse<string>> GetFundingTemplate(string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));

            string url = $"templates/{templateVersion}";

            return await GetAsync<string>(url);
        }

        public async Task<ApiResponse<FundingConfiguration>> SaveFundingConfiguration(string fundingStreamId, string fundingPeriodId, FundingConfigurationUpdateViewModel configuration)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
              
            string url = $"configuration/{fundingStreamId}/{fundingPeriodId}";

            return await PostAsync<FundingConfiguration, FundingConfigurationUpdateViewModel>(url, configuration);
        }

        public async Task<ApiResponse<Period>> SaveFundingPeriods()
        {
            string url = "fundingperiods";
            return await PostAsync<Period, object>(url, null);
        }

        public async Task<ApiResponse<string>> SaveFundingSchema(string schema)
        {
            Guard.IsNullOrWhiteSpace(schema, nameof(schema));
            string url = "schemas";

            return await PostAsync<string, object>(url, null);
        }

        public async Task<ApiResponse<FundingStream>> SaveFundingStream()
        {
            string url = "fundingstreams";
            return await PostAsync<FundingStream, object>(url, null);
        }

        public async Task<ApiResponse<string>> SaveFundingTemplate(string templateJson)
        {
            Guard.IsNullOrWhiteSpace(templateJson, nameof(templateJson));

            string url = "templates";
            return await PostAsync<string, object>(url, null);
        }
    }
}
