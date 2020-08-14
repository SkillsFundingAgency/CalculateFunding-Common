using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Policies.Models.ViewModels;
using CalculateFunding.Common.TemplateMetadata.Models;

namespace CalculateFunding.Common.ApiClient.Policies
{
    public interface IPoliciesApiClient
    {
        Task<ApiResponse<FundingConfiguration>> GetFundingConfiguration(string fundingStreamId, string fundingPeriodId);
        Task<ApiResponse<FundingConfiguration>> SaveFundingConfiguration(string fundingStreamId, string fundingPeriodId, FundingConfigurationUpdateViewModel configuration);
        Task<ApiResponse<IEnumerable<FundingPeriod>>> GetFundingPeriods();
        Task<ApiResponse<FundingPeriod>> GetFundingPeriodById(string fundingPeriodId);
        Task<ApiResponse<FundingPeriod>> SaveFundingPeriods(FundingPeriodsUpdateModel fundingPeriodsModel);
        Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreams();
        Task<ApiResponse<FundingStream>> GetFundingStreamById(string fundingStreamId);
        Task<ApiResponse<FundingStream>> SaveFundingStream(FundingStreamUpdateModel fundingStream);
        Task<ApiResponse<string>> GetFundingSchemaByVersion(string schemaVersion);
        Task<ApiResponse<string>> SaveFundingSchema(string schema);
        Task<ApiResponse<FundingTemplateContents>> GetFundingTemplate(string fundingStreamId, string fundingPeriodId, string templateVersion);
        Task<ApiResponse<string>> SaveFundingTemplate(string templateJson,string fundingStreamId, string fundingPeriodId, string templateVersion);
        Task<ApiResponse<string>> GetFundingTemplateSourceFile(string fundingStreamId, string fundingPeriodId, string templateVersion);
        Task<ApiResponse<TemplateMetadataContents>> GetFundingTemplateContents(string fundingStreamId,
            string fundingPeriodId,
            string templateVersion,
            string etag = null);
        Task<ApiResponse<IEnumerable<FundingConfiguration>>> GetFundingConfigurationsByFundingStreamId(string fundingStreamId);
        Task<ApiResponse<IEnumerable<PublishedFundingTemplate>>> GetFundingTemplates(string fundingStreamId, string fundingPeriodId);
        Task<ApiResponse<FundingDate>> GetFundingDate(string fundingStreamId, string fundingPeriodId, string fundingLineId);
        Task<ApiResponse<FundingDate>> SaveFundingDate(
            string fundingStreamId, 
            string fundingPeriodId, 
            string fundingLineId,
            FundingDateUpdateViewModel configuration);
    }
}
