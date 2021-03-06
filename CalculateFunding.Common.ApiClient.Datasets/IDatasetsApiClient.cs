using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Datasets.Models;
using CalculateFunding.Common.ApiClient.DataSets.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models.Search;

namespace CalculateFunding.Common.ApiClient.DataSets
{
    public interface IDatasetsApiClient
    {
        Task<HttpStatusCode> SaveDefinition(string yaml, string yamlFileName);
        Task<ApiResponse<IEnumerable<DatasetDefinition>>> GetDatasetDefinitions();
        Task<ApiResponse<DatasetViewModel>> GetDatasetByDatasetId(string datasetId);
        Task<ApiResponse<DatasetDefinition>> GetDatasetDefinitionById(string datasetDefinitionId);
        Task<ApiResponse<IEnumerable<DatasetDefinition>>> GetDatasetDefinitionsByIds(params string[] definitionIds);
        Task<ValidatedApiResponse<NewDatasetVersionResponseModel>> CreateNewDataset(CreateNewDatasetModel createNewDatasetModel);
        Task<ValidatedApiResponse<NewDatasetVersionResponseModel>> DatasetVersionUpdate(DatasetVersionUpdateModel datasetVersionUpdateModel);
        Task<ApiResponse<SearchResults<DatasetIndex>>> SearchDatasets(SearchModel searchModel);
        Task<ApiResponse<SearchResults<DatasetVersionIndex>>> SearchDatasetVersion(SearchModel searchModel);
        Task<ApiResponse<SearchResults<DatasetDefinitionIndex>>> SearchDatasetDefinitions(SearchModel searchModel);
        Task<ValidatedApiResponse<DatasetValidationStatusModel>> ValidateDataset(GetDatasetBlobModel getDatasetBlobModel);

        Task<ApiResponse<DefinitionSpecificationRelationship>> CreateRelationship(
            CreateDefinitionSpecificationRelationshipModel createDefinitionSpecificationRelationshipModel);
        Task<NoValidatedContentApiResponse> ValidateDefinitionSpecificationRelationship(
            ValidateDefinitionSpecificationRelationshipModel validateDefinitionSpecificationRelationshipModel);

        Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetRelationshipsBySpecificationId(string specificationId);
        Task<ApiResponse<DefinitionSpecificationRelationship>> GetRelationshipBySpecificationIdAndName(string specificationId, string name);
        Task<ApiResponse<IEnumerable<DatasetViewModel>>> GetDatasetsByDefinitionId(string definitionId);
        Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetReferenceRelationshipsBySpecificationId(string specificationId);
        Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetCurrentRelationshipsBySpecificationId(string specificationId);
        Task<ApiResponse<SelectDatasourceModel>> GetDataSourcesByRelationshipId(string relationshipId);
        Task<ApiResponse<JobCreationResponse>> AssignDatasourceVersionToRelationship(AssignDatasourceModel assignDatasourceModel);
        Task<ApiResponse<DatasetDownloadModel>> DownloadDatasetFile(string datasetId, string datasetVersion = null);
        Task<HttpStatusCode> UploadDatasetFile(string filename, DatasetMetadataViewModel datasetMetadataViewModel);
        Task<ApiResponse<DatasetDownloadModel>> DownloadDatasetMergeFile(string datasetId, string datasetVersion);
        Task<ApiResponse<string>> Reindex();
        Task<ApiResponse<string>> ReindexDatasetVersions();
        Task<ApiResponse<DatasetVersionResponseViewModel>> GetCurrentDatasetVersionByDatasetId(string datasetId);
        Task<ApiResponse<DatasetSchemaSasUrlResponseModel>> GetDatasetSchemaSasUrl(DatasetSchemaSasUrlRequestModel datasetSchemaSasUrlRequestModel);
        Task<ApiResponse<IEnumerable<DefinitionSpecificationRelationship>>> RegenerateProviderSourceDatasets(string specificationId = null);
        Task<ApiResponse<DatasetValidationStatusModel>> GetValidateDatasetStatus(string operationId);
        Task<ApiResponse<IEnumerable<DatasetAggregations>>> GetDatasetAggregationsBySpecificationId(string specificationId);
        Task<ApiResponse<IEnumerable<string>>> GetSpecificationIdsForRelationshipDefinitionId(string datasetDefinitionId);

        Task<ApiResponse<IEnumerable<DatasetSchemaRelationshipModel>>> GetDatasetSchemaRelationshipModelsForSpecificationId(string specificationId);

        Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetCurrentRelationshipsBySpecificationIdAndDatasetDefinitionId(
            string specificationId, 
            string datasetDefinitionId);

        Task<ApiResponse<IEnumerable<DatasetDefinitionByFundingStream>>> GetDatasetDefinitionsByFundingStreamId(string fundingStreamId);

        Task<ApiResponse<DatasetValidationErrorSasUrlResponseModel>> GetValidateDatasetValidationErrorSasUrl(DatasetValidationErrorRequestModel requestModel);
        Task<ApiResponse<JobCreationResponse>> QueueSpecificationConverterMergeJob(SpecificationConverterMergeRequest request);
        Task<ApiResponse<JobCreationResponse>> QueueConverterMergeJob(ConverterMergeRequest request);
        Task<ApiResponse<DatasetDownloadModel>> DownloadConverterWizardReportFile(string specificationId);
        Task<ApiResponse<ConverterDataMergeLog>> GetConverterDataMergeLog(string id);

        Task<HttpStatusCode> ToggleDatasetRelationship(string relationshipId, bool converterEnabled);

        Task<ApiResponse<IEnumerable<EligibleSpecificationReference>>> GetEligibleSpecificationsToReference(string specificationId);
        Task<ApiResponse<IEnumerable<PublishedSpecificationTemplateMetadata>>> GetPublishedSpecificationTemplateMetadata(string specificationId);
        Task<ValidatedApiResponse<DefinitionSpecificationRelationshipVersion>> UpdateDefinitionSpecificationRelationship(UpdateDefinitionSpecificationRelationshipModel model, string specificationId, string relationshipId);
        Task<ApiResponse<PublishedSpecificationConfiguration>> GetFundingLinesCalculations(string relationshipId);
    }
}