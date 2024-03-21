using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Datasets.Models;
using CalculateFunding.Common.ApiClient.DataSets.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.DataSets
{
    public class DatasetsApiClient : BaseApiClient, IDatasetsApiClient
    {
        public DatasetsApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null, string clientKey = null) 
            : base(httpClientFactory, clientKey ?? HttpClientKeys.Datasets, logger, cancellationTokenProvider)
        {
        }

        public async Task<HttpStatusCode> SaveDefinition(string yaml, string yamlFileName)
        {
            Guard.IsNullOrWhiteSpace(yaml, nameof(yaml));
            Guard.IsNullOrWhiteSpace(yamlFileName, nameof(yamlFileName));

            return await PostAsync(DataSetsUriFor("data-definitions"), CancellationToken.None, yaml, "yaml-file", yamlFileName);
        }

        public async Task<ApiResponse<IEnumerable<DatasetDefinition>>> GetDatasetDefinitions()
        {
            return await GetAsync<IEnumerable<DatasetDefinition>>(DataSetsUriFor("get-data-definitions"));
        }

        public async Task<ApiResponse<DatasetDefinition>> GetDatasetDefinitionById(string datasetDefinitionId)
        {
            Guard.IsNullOrWhiteSpace(datasetDefinitionId, nameof(datasetDefinitionId));

            return await GetAsync<DatasetDefinition>(DataSetsUriFor($"get-dataset-definition-by-id?datasetDefinitionId={datasetDefinitionId}"));
        }

        public async Task<ApiResponse<IEnumerable<DatasetDefinition>>> GetDatasetDefinitionsByIds(params string[] definitionIds)
        {
            Guard.IsNotEmpty(definitionIds, nameof(definitionIds));

            return await PostAsync<IEnumerable<DatasetDefinition>, IEnumerable<string>>(DataSetsUriFor("get-dataset-definitions-by-ids"), definitionIds);
        }

        public async Task<ApiResponse<DatasetViewModel>> GetDatasetByDatasetId(string datasetId)
        {
            Guard.IsNotEmpty(datasetId, nameof(datasetId));

            return await GetAsync<DatasetViewModel>(DataSetsUriFor($"get-dataset-by-datasetid?datasetId={datasetId}"));
        }

        public async Task<ValidatedApiResponse<NewDatasetVersionResponseModel>> CreateNewDataset(CreateNewDatasetModel createNewDatasetModel)
        {
            Guard.ArgumentNotNull(createNewDatasetModel, nameof(createNewDatasetModel));

            return await ValidatedPostAsync<NewDatasetVersionResponseModel, CreateNewDatasetModel>(DataSetsUriFor("create-new-dataset"), createNewDatasetModel);
        }

        public async Task<ValidatedApiResponse<NewDatasetVersionResponseModel>> CreateAndPersistNewDataset(CreateNewDatasetModel createNewDatasetModel)
        {
            Guard.ArgumentNotNull(createNewDatasetModel, nameof(createNewDatasetModel));

            return await ValidatedPostAsync<NewDatasetVersionResponseModel, CreateNewDatasetModel>(DataSetsUriFor("create-persist-new-dataset"), createNewDatasetModel);
        }

        public async Task<ValidatedApiResponse<NewDatasetVersionResponseModel>> DatasetVersionUpdate(DatasetVersionUpdateModel datasetVersionUpdateModel)
        {
            Guard.ArgumentNotNull(datasetVersionUpdateModel, nameof(datasetVersionUpdateModel));
            
            return await ValidatedPostAsync<NewDatasetVersionResponseModel, DatasetVersionUpdateModel>(DataSetsUriFor("dataset-version-update"), datasetVersionUpdateModel);
        }

        public async Task<ValidatedApiResponse<NewDatasetVersionResponseModel>> DatasetVersionUpdateAndPersist(DatasetVersionUpdateModel datasetVersionUpdateModel)
        {
            Guard.ArgumentNotNull(datasetVersionUpdateModel, nameof(datasetVersionUpdateModel));

            return await ValidatedPostAsync<NewDatasetVersionResponseModel, DatasetVersionUpdateModel>(DataSetsUriFor("dataset-version-update-and-persist"), datasetVersionUpdateModel);
        }

        public async Task<ApiResponse<SearchResults<DatasetIndex>>> SearchDatasets(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            return await PostAsync<SearchResults<DatasetIndex>, SearchModel>(DataSetsUriFor("datasets-search"), searchModel);
        }

        public async Task<ApiResponse<SearchResults<DatasetVersionIndex>>> SearchDatasetVersion(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));
            
            return await PostAsync<SearchResults<DatasetVersionIndex>, SearchModel>(DataSetsUriFor("datasets-version-search"), searchModel);
        }

        public async Task<ApiResponse<SearchResults<DatasetDefinitionIndex>>> SearchDatasetDefinitions(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            return await PostAsync<SearchResults<DatasetDefinitionIndex>, SearchModel>(DataSetsUriFor("dataset-definitions-search"), searchModel);
        }

        public async Task<ValidatedApiResponse<DatasetValidationStatusModel>> ValidateDataset(GetDatasetBlobModel getDatasetBlobModel)
        {
            Guard.ArgumentNotNull(getDatasetBlobModel, nameof(getDatasetBlobModel));

            return await ValidatedPostAsync<DatasetValidationStatusModel, GetDatasetBlobModel>(DataSetsUriFor("validate-dataset"), getDatasetBlobModel);
        }

        public async Task<ApiResponse<DefinitionSpecificationRelationship>> CreateRelationship(
            CreateDefinitionSpecificationRelationshipModel createDefinitionSpecificationRelationshipModel)
        {
            Guard.ArgumentNotNull(createDefinitionSpecificationRelationshipModel, nameof(createDefinitionSpecificationRelationshipModel));

            return await PostAsync<DefinitionSpecificationRelationship, CreateDefinitionSpecificationRelationshipModel>(
                DataSetsUriFor("create-definitionspecification-relationship"), 
                createDefinitionSpecificationRelationshipModel);
        }

        public async Task<ValidatedApiResponse<bool>> ValidateDefinitionSpecificationRelationship(
            ValidateDefinitionSpecificationRelationshipModel validateDefinitionSpecificationRelationshipModel)
        {
            Guard.ArgumentNotNull(validateDefinitionSpecificationRelationshipModel, nameof(validateDefinitionSpecificationRelationshipModel));

            return await ValidatedPostAsync<bool, ValidateDefinitionSpecificationRelationshipModel>(
                DataSetsUriFor("validate-definitionspecification-relationship"),
                validateDefinitionSpecificationRelationshipModel);
        }

        public async Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetRelationshipsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<DatasetSpecificationRelationshipViewModel>>(DataSetsUriFor($"get-definitions-relationships?specificationId={specificationId}"));
        }
        
        public async Task<ApiResponse<DefinitionSpecificationRelationship>> GetRelationshipBySpecificationIdAndName(string specificationId, string name)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(name, nameof(name));
            
            return await GetAsync<DefinitionSpecificationRelationship>(
                DataSetsUriFor($"get-definition-relationship-by-specificationid-name?specificationId={specificationId}&name={name}"));
        }

        public async Task<ApiResponse<IEnumerable<DatasetViewModel>>> GetDatasetsByDefinitionId(string definitionId)
        {
            Guard.IsNullOrWhiteSpace(definitionId, nameof(definitionId));

            return await GetAsync<IEnumerable<DatasetViewModel>>(DataSetsUriFor($"get-datasets-by-definitionid?definitionId={definitionId}"));
        }

        public async Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetReferenceRelationshipsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<DatasetSpecificationRelationshipViewModel>>(DataSetsUriFor($"get-reference-relationships-by-specificationId?specificationId={specificationId}"));
        }
        
        public async Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetCurrentRelationshipsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<DatasetSpecificationRelationshipViewModel>>(DataSetsUriFor($"get-relationships-by-specificationId?specificationId={specificationId}"));
        }

        public async Task<ApiResponse<SelectDatasourceModel>> GetDataSourcesByRelationshipId(string relationshipId, int? top, int? pageNumber)
        {
            Guard.IsNullOrWhiteSpace(relationshipId, nameof(relationshipId));

            string dataSourcesQueryUri = $"get-datasources-by-relationshipid?relationshipId={relationshipId}";

            if (top.HasValue)
            {
                dataSourcesQueryUri += $"&top={top}";
            }

            if (pageNumber.HasValue)
            {
                dataSourcesQueryUri += $"&pageNumber={pageNumber}";
            }

            return await GetAsync<SelectDatasourceModel>(DataSetsUriFor(dataSourcesQueryUri));
        }

        public async Task<ApiResponse<JobCreationResponse>> AssignDatasourceVersionToRelationship(AssignDatasourceModel assignDatasourceModel)
        {
            Guard.ArgumentNotNull(assignDatasourceModel, nameof(assignDatasourceModel));

            return await PostAsync<JobCreationResponse, AssignDatasourceModel>(
                DataSetsUriFor("assign-datasource-to-relationship"), 
                assignDatasourceModel);
        }

        public async Task<ApiResponse<UpdateFDSDataSchemaResponseModel>> UpdateFDSDatasetSchemaVersionOfRelationship(UpdateFDSDatasetSchemaVersionModel updateFDSDatasetSchemaVersionModel)
        {
            Guard.ArgumentNotNull(updateFDSDatasetSchemaVersionModel, nameof(updateFDSDatasetSchemaVersionModel));

            return await PostAsync<UpdateFDSDataSchemaResponseModel, UpdateFDSDatasetSchemaVersionModel>(
                DataSetsUriFor("update-fds-dataschema-version-of-relationship"),
                updateFDSDatasetSchemaVersionModel);
        }

        public async Task<ApiResponse<DatasetComparisonResponseModel>> GetFDSDatasetComparisonResult(DatasetComparisonRequestModel datasetUpgradeModel)
        {
            Guard.ArgumentNotNull(datasetUpgradeModel, nameof(datasetUpgradeModel));

            return await PostAsync<DatasetComparisonResponseModel, DatasetComparisonRequestModel>(
                DataSetsUriFor("get-fds-dataschema-comparision-Results"),
                datasetUpgradeModel);
        }

        public async Task<ApiResponse<DatasetDownloadModel>> DownloadDatasetFile(string datasetId, string datasetVersion = null)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));

            string uri = DataSetsUriFor($"download-dataset-file?datasetId={datasetId}");

            uri = datasetVersion == null ? uri : $"{uri}&datasetVersion={datasetVersion}";
 
            return await GetAsync<DatasetDownloadModel>(uri);
        }

        public async Task<ApiResponse<DatasetDownloadModel>> DownloadDatasetMergeFile(string datasetId, string datasetVersion)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));

            string uri = DataSetsUriFor($"download-dataset-merge-file?datasetId={datasetId}&datasetVersion={datasetVersion}");
 
            return await GetAsync<DatasetDownloadModel>(uri);
        }

        public async Task<ApiResponse<string>> Reindex()
        {
            return await GetAsync<string>(DataSetsUriFor("reindex"));
        }

        public async Task<ApiResponse<string>> ReindexDatasetVersions()
        {
            return await GetAsync<string>("datasetsversions/reindex");
        }

        public async Task<ApiResponse<DatasetVersionResponseViewModel>> GetCurrentDatasetVersionByDatasetId(string datasetId)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));

            return await GetAsync<DatasetVersionResponseViewModel>(DataSetsUriFor($"get-currentdatasetversion-by-datasetid?datasetId={datasetId}"));
        }

        public async Task<ApiResponse<DatasetSchemaSasUrlResponseModel>> GetDatasetSchemaSasUrl(DatasetSchemaSasUrlRequestModel datasetSchemaSasUrlRequestModel)
        {
            Guard.ArgumentNotNull(datasetSchemaSasUrlRequestModel, nameof(datasetSchemaSasUrlRequestModel));

            return await PostAsync<DatasetSchemaSasUrlResponseModel, DatasetSchemaSasUrlRequestModel>(
                DataSetsUriFor("get-schema-download-url"), datasetSchemaSasUrlRequestModel);
        }

        public async Task<ApiResponse<IEnumerable<DefinitionSpecificationRelationship>>> RegenerateProviderSourceDatasets(string specificationId = null)
        {
            string uri = DataSetsUriFor("regenerate-providersourcedatasets");

            uri = specificationId == null ? uri : $"{uri}?specificationId={specificationId}";

            return await PostAsync<IEnumerable<DefinitionSpecificationRelationship>>(uri);
        }

        public async Task<ApiResponse<DatasetValidationStatusModel>> GetValidateDatasetStatus(string operationId)
        {
            Guard.IsNullOrWhiteSpace(operationId, nameof(operationId));

            return await GetAsync<DatasetValidationStatusModel>(DataSetsUriFor($"get-dataset-validate-status?operationId={operationId}"));
        }

        public async Task<ApiResponse<IEnumerable<DatasetAggregations>>> GetDatasetAggregationsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<DatasetAggregations>>(DataSetsUriFor($"{specificationId}/datasetAggregations")); 
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetSpecificationIdsForRelationshipDefinitionId(string datasetDefinitionId)
        {
            Guard.IsNullOrWhiteSpace(datasetDefinitionId, nameof(datasetDefinitionId));

            return await GetAsync<IEnumerable<string>>(DataSetsUriFor($"{datasetDefinitionId}/relationshipSpecificationIds"));
        }

        public async Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetCurrentRelationshipsBySpecificationIdAndDatasetDefinitionId(
            string specificationId, 
            string datasetDefinitionId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(datasetDefinitionId, nameof(datasetDefinitionId));

            return await GetAsync<IEnumerable<DatasetSpecificationRelationshipViewModel>>(DataSetsUriFor($"{specificationId}/{datasetDefinitionId}/relationships"));
        }
        
        public async Task<HttpStatusCode> UploadDatasetFile(string filename, DatasetMetadataViewModel datasetMetadataViewModel)
        {
            Guard.IsNullOrWhiteSpace(filename, nameof(filename));
            Guard.ArgumentNotNull(datasetMetadataViewModel, nameof(datasetMetadataViewModel));

            return await PostAsync(DataSetsUriFor($"upload-dataset-file/{filename}"), datasetMetadataViewModel);
        }

        public async Task<HttpStatusCode> UploadDatasetFileRaw(string filename, DatasetMetadataViewModelRaw datasetMetadataViewModelRaw)
        {
            Guard.IsNullOrWhiteSpace(filename, nameof(filename));
            Guard.ArgumentNotNull(datasetMetadataViewModelRaw, nameof(datasetMetadataViewModelRaw));

            return await PostAsync(DataSetsUriFor($"upload-raw-dataset-file/{filename}"), datasetMetadataViewModelRaw);
        }

        public async Task<HttpStatusCode> ToggleDatasetRelationship(string relationshipId, bool converterEnabled)
        {
            Guard.IsNullOrWhiteSpace(relationshipId, nameof(relationshipId));

            return await PutAsync(DataSetsUriFor($"toggleDatasetSchema/{relationshipId}"), converterEnabled);
        }

        public async Task<ApiResponse<IEnumerable<DatasetSchemaRelationshipModel>>> GetDatasetSchemaRelationshipModelsForSpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<DatasetSchemaRelationshipModel>>(DataSetsUriFor($"{specificationId}/schemaRelationshipFields"));
        }

        public async Task<ApiResponse<IEnumerable<DatasetDefinitionByFundingStream>>> GetDatasetDefinitionsByFundingStreamId(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<DatasetDefinitionByFundingStream>>(DataSetsUriFor($"get-data-definitions/{fundingStreamId}"));
        }

        public async Task<ApiResponse<JobCreationResponse>> QueueSpecificationConverterMergeJob(SpecificationConverterMergeRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await PostAsync<JobCreationResponse, SpecificationConverterMergeRequest>(
                "specifications/datasets/converter/merge", request);
        }

        public async Task<ApiResponse<JobCreationResponse>> QueueConverterMergeJob(ConverterMergeRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await PostAsync<JobCreationResponse, ConverterMergeRequest>(
                "datasets/converter/merge", request);
        }

        public async Task<ApiResponse<JobCreationResponse>> QueueProcessDatasetObsoleteItemsJob(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<JobCreationResponse>(
                DataSetsUriFor($"queue-process-dataset-obsolete-items-job/{specificationId}"));
        }

        private string DataSetsUriFor(string relativeUri)
        {
            return $"datasets/{relativeUri}";
        }

        public async Task<ApiResponse<DatasetValidationErrorSasUrlResponseModel>> GetValidateDatasetValidationErrorSasUrl(
            DatasetValidationErrorRequestModel requestModel)
        {
            Guard.ArgumentNotNull(requestModel, nameof(requestModel));

            return await PostAsync<DatasetValidationErrorSasUrlResponseModel, DatasetValidationErrorRequestModel>(
                DataSetsUriFor("get-validate-dataset-error-url"), requestModel);
        }

        public async Task<ApiResponse<DatasetDownloadModel>> DownloadConverterWizardReportFile(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string uri = DataSetsUriFor($"reports/{specificationId}/report-metadata");

            return await GetAsync<DatasetDownloadModel>(uri);
        }

        public async Task<ApiResponse<ConverterDataMergeLog>> GetConverterDataMergeLog(string id)
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));

            return await GetAsync<ConverterDataMergeLog>($"reports/converter-data-merge-log/{id}");
        }

        public async Task<ApiResponse<IEnumerable<EligibleSpecificationReference>>> GetEligibleSpecificationsToReference(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<EligibleSpecificationReference>>($"specifications/{specificationId}/eligible-specification-references");
        }

        public async Task<ApiResponse<IEnumerable<PublishedSpecificationTemplateMetadata>>> GetPublishedSpecificationTemplateMetadata(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<PublishedSpecificationTemplateMetadata>>($"specifications/{specificationId}/published-specification-template-metadata");
        }

        public async Task<ValidatedApiResponse<DefinitionSpecificationRelationshipVersion>> UpdateDefinitionSpecificationRelationship(UpdateDefinitionSpecificationRelationshipModel model, string specificationId, string relationshipId)
        {
            Guard.ArgumentNotNull(model, nameof(model));
            Guard.IsNullOrWhiteSpace(relationshipId, nameof(relationshipId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPutAsync<DefinitionSpecificationRelationshipVersion, UpdateDefinitionSpecificationRelationshipModel>($"specifications/{specificationId}/datasets/edit-definition-specification-relationship/{relationshipId}", model);
        }

        public async Task<ApiResponse<PublishedSpecificationConfiguration>> GetFundingLinesCalculations(string relationshipId)
        {
            Guard.IsNullOrWhiteSpace(relationshipId, nameof(relationshipId));

            return await GetAsync<PublishedSpecificationConfiguration>($"datasets/definition-relationships/{relationshipId}/get-funding-line-calculations");
        }

        public async Task<ApiResponse<DatasetDefinitionViewModel>> GetNewFDSDatasetSchemaVersion(string fundingStreamCode, string fundingPeriodCode, string datasetDefinitionName)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamCode, nameof(fundingStreamCode));
            Guard.IsNullOrWhiteSpace(fundingPeriodCode, nameof(fundingPeriodCode));
            Guard.IsNullOrWhiteSpace(datasetDefinitionName, nameof(datasetDefinitionName));

            return await GetAsync<DatasetDefinitionViewModel>(DataSetsUriFor($"get-new-fds-dataschema-version?" +
                $"fundingStreamCode={fundingStreamCode}&fundingPeriodCode={fundingPeriodCode}&datasetDefinitionName={datasetDefinitionName}"));
        }
    }
}