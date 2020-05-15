using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        public DatasetsApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null) 
            : base(httpClientFactory, HttpClientKeys.Datasets, logger, cancellationTokenProvider)
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

        public async Task<ValidatedApiResponse<NewDatasetVersionResponseModel>> CreateNewDataset(CreateNewDatasetModel createNewDatasetModel)
        {
            Guard.ArgumentNotNull(createNewDatasetModel, nameof(createNewDatasetModel));

            return await ValidatedPostAsync<NewDatasetVersionResponseModel, CreateNewDatasetModel>(DataSetsUriFor("create-new-dataset"), createNewDatasetModel);
        }

        public async Task<ValidatedApiResponse<NewDatasetVersionResponseModel>> DatasetVersionUpdate(DatasetVersionUpdateModel datasetVersionUpdateModel)
        {
            Guard.ArgumentNotNull(datasetVersionUpdateModel, nameof(datasetVersionUpdateModel));
            
            return await ValidatedPostAsync<NewDatasetVersionResponseModel, DatasetVersionUpdateModel>(DataSetsUriFor("dataset-version-update"), datasetVersionUpdateModel);
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

        public async Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetRelationshipsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<DatasetSpecificationRelationshipViewModel>>(DataSetsUriFor($"get-definitions-relationships?specificationId={specificationId}"));
        }
        
        public async Task<ApiResponse<IEnumerable<DefinitionSpecificationRelationship>>> GetRelationshipBySpecificationIdAndName(string specificationId, string name)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(name, nameof(name));
            
            return await GetAsync<IEnumerable<DefinitionSpecificationRelationship>>(
                DataSetsUriFor($"get-definition-relationship-by-specificationid-name?specificationId={specificationId}&name={name}"));
        }

        public async Task<ApiResponse<IEnumerable<DatasetViewModel>>> GetDatasetsByDefinitionId(string definitionId)
        {
            Guard.IsNullOrWhiteSpace(definitionId, nameof(definitionId));

            return await GetAsync<IEnumerable<DatasetViewModel>>(DataSetsUriFor($"get-datasets-by-definitionid?definitionId={definitionId}"));
        }

        public async Task<ApiResponse<IEnumerable<DatasetSpecificationRelationshipViewModel>>> GetCurrentRelationshipsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<DatasetSpecificationRelationshipViewModel>>(DataSetsUriFor($"get-relationships-by-specificationId?specificationId={specificationId}"));
        }

        public async Task<ApiResponse<SelectDatasourceModel>> GetDataSourcesByRelationshipId(string relationshipId)
        {
            Guard.IsNullOrWhiteSpace(relationshipId, nameof(relationshipId));

            return await GetAsync<SelectDatasourceModel>(DataSetsUriFor($"get-datasources-by-relationshipid?relationshipId={relationshipId}"));
        }

        public async Task<HttpStatusCode> AssignDatasourceVersionToRelationship(AssignDatasourceModel assignDatasourceModel)
        {
            Guard.ArgumentNotNull(assignDatasourceModel, nameof(assignDatasourceModel));

            return await PostAsync(DataSetsUriFor("assign-datasource-to-relationship"), assignDatasourceModel);
        }

        public async Task<ApiResponse<DatasetDownloadModel>> DownloadDatasetFile(string datasetId, string datasetVersion = null)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));

            string uri = DataSetsUriFor($"download-dataset-file?datasetId={datasetId}");

            uri = datasetVersion == null ? uri : $"{uri}&datasetVersion={datasetVersion}";
 
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

            return await PostAsync($"upload-dataset-file/{filename}", datasetMetadataViewModel);
        }

        public async Task<ApiResponse<IEnumerable<DatasetSchemaRelationshipModel>>> GetDatasetSchemaRelationshipModelsForSpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<DatasetSchemaRelationshipModel>>(DataSetsUriFor($"{specificationId}/schemaRelationshipFields"));
        }

        private string DataSetsUriFor(string relativeUri)
        {
            return $"datasets/{relativeUri}";
        }
    }
}