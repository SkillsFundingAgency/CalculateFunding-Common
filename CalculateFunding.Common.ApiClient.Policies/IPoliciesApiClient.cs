using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Policies.Models.ViewModels;
using CalculateFunding.Common.TemplateMetadata.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Policies
{
    public interface IPoliciesApiClient
    {
        Task<ApiResponse<FundingConfiguration>> GetFundingConfiguration(string fundingStreamId, string fundingPeriodId);
        Task<ApiResponse<FundingConfiguration>> SaveFundingConfiguration(string fundingStreamId, string fundingPeriodId, FundingConfigurationUpdateViewModel configuration);
        Task<ApiResponse<IEnumerable<Period>>> GetFundingPeriods();
        Task<ApiResponse<Period>> GetFundingPeriodById(string fundingPeriodId);
        Task<ApiResponse<Period>> SaveFundingPeriods();
        Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreams();
        Task<ApiResponse<FundingStream>> GetFundingStreamById(string fundingStreamId);
        Task<ApiResponse<FundingStream>> SaveFundingStream();
        Task<ApiResponse<string>> GetFundingSchemaByVersion(string schemaVersion);
        Task<ApiResponse<string>> SaveFundingSchema(string schema);
        Task<ApiResponse<FundingTemplateContents>> GetFundingTemplate(string fundingStreamId, string templateVersion);
        Task<ApiResponse<string>> SaveFundingTemplate(string templateJson);
        Task<ApiResponse<string>> GetFundingTemplateSourceFile(string fundingStreamId, string templateVersion);
        Task<ApiResponse<TemplateMetadataContents>> GetFundingTemplateContents(string fundingStreamId, string templateVersion);
        Task<ApiResponse<IEnumerable<FundingConfiguration>>> GetFundingConfigurationsByFundingStreamId(string fundingStreamId);
    }
}
