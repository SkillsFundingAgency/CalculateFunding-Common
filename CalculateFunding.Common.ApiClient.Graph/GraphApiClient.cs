using CalculateFunding.Common.ApiClient.Graph.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Graph
{
    public class GraphApiClient : BaseApiClient, IGraphApiClient
    {
        private const string UrlRoot = "graph";

        public GraphApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
         : base(httpClientFactory, HttpClientKeys.Graph, logger, cancellationTokenProvider)
        { }

        public async Task<HttpStatusCode> DeleteCalculationSpecificationRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/specification/relationships/calculation/delete", relationships);
        }

        public async Task<HttpStatusCode> UpsertDataset(Dataset dataset)
        {
            Guard.ArgumentNotNull(dataset, nameof(dataset));

            return await PostAsync($"{UrlRoot}/dataset", dataset);
        }

        public async Task<HttpStatusCode> UpsertDatasets(Dataset[] datasets)
        {
            Guard.ArgumentNotNull(datasets, nameof(datasets));

            return await PostAsync($"{UrlRoot}/datasets", datasets);
        }

        public async Task<HttpStatusCode> DeleteDataset(string datasetId)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));

            return await DeleteAsync($"{UrlRoot}/datasets/{datasetId}");
        }
        
        public async Task<HttpStatusCode> UpsertCalculations(Calculation[] calculations)
        {
            Guard.ArgumentNotNull(calculations, nameof(calculations));

            string url = $"{UrlRoot}/calculations";

            return await PostAsync(url, calculations);
        }
        
        public async Task<HttpStatusCode> UpsertDatasetDefinition(DatasetDefinition definition)
        {
            Guard.ArgumentNotNull(definition, nameof(definition));

            return await PostAsync($"{UrlRoot}/datasetdefinition",
                definition);
        }

        public async Task<HttpStatusCode> UpsertDatasetDefinitions(DatasetDefinition[] definitions)
        {
            Guard.ArgumentNotNull(definitions, nameof(definitions));

            return await PostAsync($"{UrlRoot}/datasetdefinitions", definitions);
        }

        public async Task<HttpStatusCode> DeleteDatasetDefinition(string definitionId)
        {
            Guard.IsNullOrWhiteSpace(definitionId, nameof(definitionId));
            
            return await DeleteAsync($"{UrlRoot}/datasetdefinitions/{definitionId}");
        }

        public async Task<HttpStatusCode> UpsertDataField(DataField dataField)
        {
            Guard.ArgumentNotNull(dataField, nameof(dataField));

            return await PostAsync($"{UrlRoot}/datafield",
                dataField);
        }

        public async Task<HttpStatusCode> UpsertDataFields(DataField[] dataFields)
        {
            Guard.ArgumentNotNull(dataFields, nameof(dataFields));

            return await PostAsync($"{UrlRoot}/datafields",
                dataFields);
        }

        public async Task<HttpStatusCode> DeleteDataField(string dataFieldId)
        {
            Guard.IsNullOrWhiteSpace(dataFieldId, nameof(dataFieldId));

            return await DeleteAsync($"{UrlRoot}/datafields/{dataFieldId}");
        }
        
        public async Task<HttpStatusCode> UpsertDataDefinitionDatasetRelationship(string definitionId, string datasetId)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));
            
            return await PutAsync($"{UrlRoot}/datasetdefinitions/{definitionId}/relationships/datasets/{datasetId}");
        }
        
        public async Task<HttpStatusCode> UpsertDataDefinitionDatasetRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));
            
            return await PostAsync($"{UrlRoot}/datasetdefinitions/relationships/datasets", relationships);
        }

        public async Task<HttpStatusCode> DeleteDataDefinitionDatasetRelationship(string definitionId, string datasetId)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));
            
            return await DeleteAsync($"{UrlRoot}/datasetdefinitions/{definitionId}/relationships/datasets/{datasetId}");
        }

        public async Task<HttpStatusCode> UpsertDatasetDataFieldRelationship(string datasetId, string fieldId)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));
            
            return await PutAsync($"{UrlRoot}/datasets/{datasetId}/relationships/datafields/{fieldId}");
        }
        
        public async Task<HttpStatusCode> UpsertDatasetDataFieldRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));
            
            return await PostAsync($"{UrlRoot}/datasets/relationships/datafields", relationships);
        }
        
        public async Task<HttpStatusCode> DeleteDatasetDataFieldRelationship(string datasetId, string fieldId)
        {
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));
            
            return await DeleteAsync($"{UrlRoot}/datasets/{datasetId}/relationships/datafields/{fieldId}");
        }
        
        public async Task<HttpStatusCode> UpsertSpecificationDatasetRelationship(string specificationId, string datasetId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));

            return await PutAsync($"{UrlRoot}/specifications/{specificationId}/relationships/datasets/{datasetId}");
        }

        public async Task<HttpStatusCode> UpsertSpecificationDatasetRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/specifications/relationships/datasets", relationships);
        }

        public async Task<HttpStatusCode> DeleteSpecificationDatasetRelationship(string specificationId, string datasetId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));

            return await DeleteAsync($"{UrlRoot}/specifications/{specificationId}/relationships/datasets/{datasetId}");
        }

        public async Task<HttpStatusCode> DeleteSpecificationDatasetRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/specifications/relationships/datasets/delete", relationships);
        }

        public async Task<HttpStatusCode> DeleteCalculationDataFieldRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/calculations/relationships/datafields/delete", relationships);
        }

        public async Task<HttpStatusCode> UpsertCalculationDataFieldRelationship(string calculationId, string fieldId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));

            return await PutAsync($"{UrlRoot}/calculations/{calculationId}/relationships/datafields/{fieldId}");
        }

        public async Task<HttpStatusCode> UpsertCalculationDataFieldRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PutAsync($"{UrlRoot}/calculations/relationships/datafields", relationships);
        }

        public async Task<HttpStatusCode> DeleteCalculationDataFieldRelationship(string calculationId, string fieldId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));

            return await DeleteAsync($"{UrlRoot}/calculations/{calculationId}/relationships/datafields/{fieldId}");
        }
        
        public async Task<HttpStatusCode> UpsertSpecifications(Specification[] specifications)
        {
            Guard.ArgumentNotNull(specifications, nameof(specifications));

            return await PostAsync($"{UrlRoot}/specifications", specifications);
        }

        public async Task<HttpStatusCode> DeleteCalculation(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            return await DeleteAsync($"{UrlRoot}/calculation/{calculationId}");
        }

        public async Task<HttpStatusCode> DeleteCalculations(params string[] calculationIds)
        {
            Guard.ArgumentNotNull(calculationIds, nameof(calculationIds));

            return await PostAsync($"{UrlRoot}/calculation/delete", calculationIds);
        }

        public async Task<HttpStatusCode> DeleteSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await DeleteAsync($"{UrlRoot}/specification/{specificationId}");
        }

        public async Task<HttpStatusCode> UpsertCalculationCalculationsRelationships(string calculationId, string[] calculationIds)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.ArgumentNotNull(calculationIds, nameof(calculationIds));

            string url = $"{UrlRoot}/calculation/{calculationId}/relationships/calculations";

            return await PostAsync(url, calculationIds);
        }
        public async Task<HttpStatusCode> UpsertFundingLines(FundingLine[] fundingLines)
        {
            Guard.ArgumentNotNull(fundingLines, nameof(fundingLines));

            string url = $"{UrlRoot}/fundinglines";

            return await PostAsync(url, fundingLines);
        }

        public async Task<HttpStatusCode> DeleteFundingLine(string fieldId)
        {
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));

            return await DeleteAsync($"{UrlRoot}/fundingline/{fieldId}");
        }

        public async Task<HttpStatusCode> DeleteFundingLines(params string[] fieldIds)
        {
            Guard.ArgumentNotNull(fieldIds, nameof(fieldIds));

            return await PostAsync($"{UrlRoot}/fundingline/delete", fieldIds);
        }

        public async Task<HttpStatusCode> UpsertFundingLineCalculationRelationship(string fundingLineId, string calculationId)
        {
            Guard.IsNullOrWhiteSpace(fundingLineId, nameof(fundingLineId));
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            return await PutAsync($"{UrlRoot}/fundingline/{fundingLineId}/relationships/calculation/{calculationId}");
        }

        public async Task<HttpStatusCode> UpsertFundingLineCalculationRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/fundingline/relationships/calculation", relationships);
        }

        public async Task<HttpStatusCode> UpsertCalculationFundingLineRelationship(string calculationId, string fundingLineId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(fundingLineId, nameof(fundingLineId));

            return await PutAsync($"{UrlRoot}/calculation/{calculationId}/relationships/fundingline/{fundingLineId}");
        }

        public async Task<HttpStatusCode> UpsertCalculationFundingLineRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/calculation/relationships/fundingline", relationships);
        }

        public async Task<HttpStatusCode> DeleteFundingLineCalculationRelationship(string fundingLineId, string calculationId)
        {
            Guard.IsNullOrWhiteSpace(fundingLineId, nameof(fundingLineId));
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            return await DeleteAsync($"{UrlRoot}/fundingline/{fundingLineId}/relationships/calculation/{calculationId}");
        }

        public async Task<HttpStatusCode> DeleteFundingLineCalculationRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/fundingline/relationships/calculation/delete", relationships);
        }

        public async Task<HttpStatusCode> DeleteCalculationFundingLineRelationship(string calculationId, string fundingLineId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(fundingLineId, nameof(fundingLineId));

            return await DeleteAsync($"{UrlRoot}/calculation/{calculationId}/relationships/fundingline/{fundingLineId}");
        }

        public async Task<HttpStatusCode> DeleteCalculationFundingLineRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/calculation/relationships/fundingline/delete", relationships);
        }

        public async Task<ApiResponse<IEnumerable<Entity<Calculation>>>> GetCircularDependencies(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<Entity<Calculation>>>($"{UrlRoot}/calculation/circulardependencies/{specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<Entity<Specification>>>> GetAllEntitiesRelatedToSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<Entity<Specification>>>($"{UrlRoot}/specification/getallentities/{specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<Entity<DataField>>>> GetAllEntitiesRelatedToDataset(string datafieldId)
        {
            Guard.IsNullOrWhiteSpace(datafieldId, nameof(datafieldId));

            return await GetAsync<IEnumerable<Entity<DataField>>>($"{UrlRoot}/dataset/getallentities/{datafieldId}");
        }

        public async Task<ApiResponse<IEnumerable<Entity<FundingLine>>>> GetAllEntitiesRelatedToFundingLine(string fundingLineId)
        {
            Guard.IsNullOrWhiteSpace(fundingLineId, nameof(fundingLineId));

            return await GetAsync<IEnumerable<Entity<FundingLine>>>($"{UrlRoot}/fundingline/getallentities/{fundingLineId}");
        }
        
        public async Task<ApiResponse<IEnumerable<Entity<FundingLine>>>> GetAllEntitiesRelatedToFundingLines(params string[] fundingLineIds)
        {
            Guard.ArgumentNotNull(fundingLineIds, nameof(fundingLineIds));

            return await PostAsync<IEnumerable<Entity<FundingLine>>, IEnumerable<string>>($"{UrlRoot}/fundingline/getallentitiesforall", fundingLineIds);
        }

        public async Task<ApiResponse<IEnumerable<Entity<Calculation>>>> GetAllEntitiesRelatedToCalculation(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            return await GetAsync<IEnumerable<Entity<Calculation>>>($"{UrlRoot}/calculation/getallentities/{calculationId}");
        }
        
        public async Task<ApiResponse<IEnumerable<Entity<Calculation>>>> GetAllEntitiesRelatedToCalculations(params string[] calculationIds)
        {
            Guard.ArgumentNotNull(calculationIds, nameof(calculationIds));
            
            return await PostAsync<IEnumerable<Entity<Calculation>>, IEnumerable<string>>($"{UrlRoot}/calculation/getallentitiesforall",
                calculationIds);
        }

        public async Task<HttpStatusCode> UpsertCalculationCalculationRelationship(string calculationIdA, string calculationIdB)
        {
            Guard.IsNullOrWhiteSpace(calculationIdA, nameof(calculationIdA));
            Guard.IsNullOrWhiteSpace(calculationIdB, nameof(calculationIdB));

            return await PutAsync($"{UrlRoot}/calculation/{calculationIdA}/relationships/calculation/{calculationIdB}");
        }

        public async Task<HttpStatusCode> UpsertCalculationSpecificationRelationship(string calculationId, string specificationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PutAsync($"{UrlRoot}/specification/{specificationId}/relationships/calculation/{calculationId}");
        }
        
        public async Task<HttpStatusCode> UpsertCalculationSpecificationRelationships(params AmendRelationshipRequestModel[] relationships)
        {
           Guard.ArgumentNotNull(relationships, nameof(relationships));

           return await PostAsync($"{UrlRoot}/specification/relationships/calculation", relationships);
        }

        public async Task<HttpStatusCode> DeleteCalculationCalculationRelationship(string calculationIdA, string calculationIdB)
        {
            Guard.IsNullOrWhiteSpace(calculationIdA, nameof(calculationIdA));
            Guard.IsNullOrWhiteSpace(calculationIdB, nameof(calculationIdB));

            return await DeleteAsync($"{UrlRoot}/calculation/{calculationIdA}/relationships/calculation/{calculationIdB}");
        }

        public async Task<HttpStatusCode> DeleteCalculationCalculationRelationships(params AmendRelationshipRequestModel[] relationships)
        {
            Guard.ArgumentNotNull(relationships, nameof(relationships));

            return await PostAsync($"{UrlRoot}/", relationships);
        }

        public async Task<HttpStatusCode> DeleteCalculationSpecificationRelationship(string calculationId, string specificationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await DeleteAsync($"{UrlRoot}/specification/{specificationId}/relationships/calculation/{calculationId}");
        }

        public async Task<HttpStatusCode> UpsertCalculationDataFieldsRelationships(string calculationId, string[] dataFieldIds)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.ArgumentNotNull(dataFieldIds, nameof(dataFieldIds));

            return await PostAsync($"{UrlRoot}/calculation/{calculationId}/relationships/datafields", dataFieldIds);
        }
    }
}
