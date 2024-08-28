using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Policies.Models.ViewModels;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Policies
{
    public class PoliciesApiClient : BaseApiClient, IPoliciesApiClient
    {
        public PoliciesApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null, string clientKey = null)
         : base(httpClientFactory, clientKey ?? HttpClientKeys.Policies, logger, cancellationTokenProvider)
        { }

        public async Task<ApiResponse<FundingConfiguration>> GetFundingConfiguration(string fundingStreamId, string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<FundingConfiguration>($"configuration/{fundingStreamId}/{fundingPeriodId}");
        }

        public async Task<ApiResponse<IEnumerable<FundingConfiguration>>> GetFundingConfigurationsByFundingStreamId(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<FundingConfiguration>>($"configuration/{fundingStreamId}");
        }

        public async Task<ApiResponse<FundingPeriod>> GetFundingPeriodById(string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<FundingPeriod>($"fundingperiods/{fundingPeriodId}");
        }

        public async Task<ApiResponse<IEnumerable<FundingPeriod>>> GetFundingPeriods()
        {
            return await GetAsync<IEnumerable<FundingPeriod>>("fundingperiods");
        }

        public async Task<ApiResponse<string>> GetFundingSchemaByVersion(string schemaVersion)
        {
            Guard.IsNullOrWhiteSpace(schemaVersion, nameof(schemaVersion));

            return await GetAsync<string>($"schemas/{schemaVersion}");
        }

        public async Task<ApiResponse<FundingStream>> GetFundingStreamById(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<FundingStream>($"fundingstreams/{fundingStreamId}");
        }

        public async Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreams()
        {
            return await GetAsync<IEnumerable<FundingStream>>("fundingstreams");
        }

        public async Task<ApiResponse<FundingTemplateContents>> GetFundingTemplate(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<FundingTemplateContents>($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}");
        }

        public async Task<ApiResponse<FundingConfiguration>> SaveFundingConfiguration(string fundingStreamId, string fundingPeriodId, FundingConfigurationUpdateViewModel configuration)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await PostAsync<FundingConfiguration, FundingConfigurationUpdateViewModel>($"configuration/{fundingStreamId}/{fundingPeriodId}", configuration);
        }

        public async Task<ApiResponse<FundingPeriod>> SaveFundingPeriods(FundingPeriodsUpdateModel fundingPeriodsUpdateModel)
        {
            Guard.ArgumentNotNull(fundingPeriodsUpdateModel, nameof(fundingPeriodsUpdateModel));

            return await PostAsync<FundingPeriod, FundingPeriodsUpdateModel>("fundingperiods", fundingPeriodsUpdateModel);           
        }

        public async Task<ApiResponse<string>> SaveFundingSchema(string schema)
        {
            Guard.IsNullOrWhiteSpace(schema, nameof(schema));

            return await PostAsync<string, object>("schemas", null);
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

            return await PostAsync<string, object>($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}", templateJson);
        }

        public async Task<ApiResponse<string>> GetFundingTemplateSourceFile(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<string>($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/sourcefile");
        }

        public async Task<ApiResponse<TemplateMetadataContents>> GetFundingTemplateContents(string fundingStreamId,
            string fundingPeriodId,
            string templateVersion,
            string etag = null)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            string url = $"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata";

            return etag.IsNullOrEmpty() 
                ? await GetAsync<TemplateMetadataContents>(url) 
                : await GetAsync<TemplateMetadataContents>(url, default, IfNoneMatch, etag);
        }

        public async Task<ApiResponse<IEnumerable<PublishedFundingTemplate>>> GetFundingTemplates(string fundingStreamId, string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));           
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<IEnumerable<PublishedFundingTemplate>>($"templates/{fundingStreamId}/{fundingPeriodId}");
        }

        public async Task<ApiResponse<FundingDate>> GetFundingDate(string fundingStreamId, string fundingPeriodId, string fundingLineId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingLineId, nameof(fundingLineId));

            return await GetAsync<FundingDate>($"fundingdates/{fundingStreamId}/{fundingPeriodId}/{fundingLineId}");
        }

        public async Task<ApiResponse<FundingDate>> SaveFundingDate(
            string fundingStreamId, 
            string fundingPeriodId, 
            string fundingLineId, 
            FundingDateUpdateViewModel configuration)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingLineId, nameof(fundingLineId));

            return await PostAsync<FundingDate, FundingDateUpdateViewModel>($"fundingdates/{fundingStreamId}/{fundingPeriodId}/{fundingLineId}", configuration);
        }

        public async Task<ApiResponse<TemplateMetadataDistinctContents>> GetDistinctTemplateMetadataContents(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));

            return await GetAsync<TemplateMetadataDistinctContents>($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata/distinct");
        }

        public async Task<ApiResponse<TemplateMetadataDistinctFundingLinesContents>> GetDistinctTemplateMetadataFundingLinesContents(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));

            return await GetAsync<TemplateMetadataDistinctFundingLinesContents>($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata/distinct/funding-lines");
        }

        public async Task<ApiResponse<TemplateMetadataDistinctCalculationsContents>> GetDistinctTemplateMetadataCalculationsContents(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));

            return await GetAsync<TemplateMetadataDistinctCalculationsContents>($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata/distinct/calculations");
        }

        public async Task<ApiResponse<TemplateMetadataFundingLineCashCalculationsContents>> GetCashCalcsForFundingLines(string fundingStreamId, string fundingPeriodId, string templateVersion)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));

            return await GetAsync<TemplateMetadataFundingLineCashCalculationsContents>($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata/cash-calculations");
        }
    }
}
