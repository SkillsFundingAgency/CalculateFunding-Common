using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Policies.Models.ViewModels;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.TemplateMetadata.Models;
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

        public async Task<ApiResponse<IEnumerable<FundingConfiguration>>> GetFundingConfigurationsByFundingStreamId(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            string url = $"configuration/{fundingStreamId}";

            return await GetAsync<IEnumerable<FundingConfiguration>>(url);
        }

        public async Task<ApiResponse<FundingPeriod>> GetFundingPeriodById(string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            string url = $"fundingperiods/{fundingPeriodId}";

            return await GetAsync<FundingPeriod>(url);
        }

        public async Task<ApiResponse<IEnumerable<FundingPeriod>>> GetFundingPeriods()
        {
            string url = "fundingperiods";

            return await GetAsync<IEnumerable<FundingPeriod>>(url);
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

        public async Task<ApiResponse<FundingTemplateContents>> GetFundingTemplate(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            string url = $"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}";

            return await GetAsync<FundingTemplateContents>(url);
        }

        public async Task<ApiResponse<FundingConfiguration>> SaveFundingConfiguration(string fundingStreamId, string fundingPeriodId, FundingConfigurationUpdateViewModel configuration)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            string url = $"configuration/{fundingStreamId}/{fundingPeriodId}";

            return await PostAsync<FundingConfiguration, FundingConfigurationUpdateViewModel>(url, configuration);
        }

        public async Task<ApiResponse<FundingPeriod>> SaveFundingPeriods(FundingPeriodsUpdateModel fundingPeriodsUpdateModel)
        {
            Guard.ArgumentNotNull(fundingPeriodsUpdateModel, nameof(fundingPeriodsUpdateModel));          

            string url = "fundingperiods";
            return await PostAsync<FundingPeriod, FundingPeriodsUpdateModel>(url, fundingPeriodsUpdateModel);           
        }

        public async Task<ApiResponse<string>> SaveFundingSchema(string schema)
        {
            Guard.IsNullOrWhiteSpace(schema, nameof(schema));
            string url = "schemas";

            return await PostAsync<string, object>(url, null);
        }

        public async Task<ApiResponse<FundingStream>> SaveFundingStream(FundingStreamUpdateModel fundingStream)
        {
            Guard.ArgumentNotNull(fundingStream, nameof(fundingStream));            

            return await PostAsync<FundingStream, FundingStreamUpdateModel>("fundingstreams", fundingStream);
        }

        public async Task<ApiResponse<string>> SaveFundingTemplate(string templateJson, string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(templateJson, nameof(templateJson));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            string url = $"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}";
            return await PostAsync<string, object>(url, templateJson);
        }

        public async Task<ApiResponse<string>> GetFundingTemplateSourceFile(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            string url = $"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/sourcefile";

            return await GetAsync<string>(url);
        }

        public async Task<ApiResponse<TemplateMetadataContents>> GetFundingTemplateContents(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            string url = $"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata";

            return await GetAsync<TemplateMetadataContents>(url);
        }
    }
}
