using CalculateFunding.Common.ApiClient.Graph.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Newtonsoft.Json.Linq;
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
        
        public async Task<HttpStatusCode> DeleteSpecificationDatasetRelationship(string specificationId, string datasetId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(datasetId, nameof(datasetId));

            return await DeleteAsync($"{UrlRoot}/specifications/{specificationId}/relationships/datasets/{datasetId}");
        }
        
        public async Task<HttpStatusCode> UpsertCalculationDataFieldRelationship(string calculationId, string fieldId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));

            return await PutAsync($"{UrlRoot}/calculations/{calculationId}/relationships/datafields/{fieldId}");
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

            string url = $"{UrlRoot}/specifications";

            return await PostAsync(url, specifications);
        }

        public async Task<HttpStatusCode> DeleteCalculation(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            string url = $"{UrlRoot}/calculation/{calculationId}";

            return await DeleteAsync(url);
        }

        public async Task<HttpStatusCode> DeleteSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/specification/{specificationId}";

            return await DeleteAsync(url);
        }

        public async Task<HttpStatusCode> UpsertCalculationCalculationsRelationships(string calculationId, string[] calculationIds)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.ArgumentNotNull(calculationIds, nameof(calculationIds));

            string url = $"{UrlRoot}/calculation/{calculationId}/relationships/calculations";

            return await PostAsync(url, calculationIds);
        }

        public async Task<ApiResponse<IEnumerable<Entity<Calculation>>>> GetCircularDependencies(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            string url = $"{UrlRoot}/calculation/circulardependencies/{specificationId}";

            return await GetAsync<IEnumerable<Entity<Calculation>>>(url);
        }

        public async Task<ApiResponse<IEnumerable<Entity<Specification>>>> GetAllEntitiesRelatedToSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            string url = $"{UrlRoot}/specification/getallentities/{specificationId}";

            return await GetAsync<IEnumerable<Entity<Specification>>>(url);
        }

        public async Task<ApiResponse<IEnumerable<Entity<Calculation>>>> GetAllEntitiesRelatedToCalculation(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            string url = $"{UrlRoot}/calculation/getallentities/{calculationId}";

            return await GetAsync<IEnumerable<Entity<Calculation>>>(url);
        }

        public async Task<HttpStatusCode> UpsertCalculationCalculationRelationship(string calculationIdA, string calculationIdB)
        {
            Guard.IsNullOrWhiteSpace(calculationIdA, nameof(calculationIdA));
            Guard.IsNullOrWhiteSpace(calculationIdB, nameof(calculationIdB));

            string url = $"{UrlRoot}/calculation/{calculationIdA}/relationships/calculation/{calculationIdB}";

            return await PutAsync(url);
        }

        public async Task<HttpStatusCode> UpsertCalculationSpecificationRelationship(string calculationId, string specificationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/specification/{specificationId}/relationships/calculation/{calculationId}";

            return await PutAsync(url);
        }

        public async Task<HttpStatusCode> DeleteCalculationCalculationRelationship(string calculationIdA, string calculationIdB)
        {
            Guard.IsNullOrWhiteSpace(calculationIdA, nameof(calculationIdA));
            Guard.IsNullOrWhiteSpace(calculationIdB, nameof(calculationIdB));

            string url = $"{UrlRoot}/calculation/{calculationIdA}/relationships/calculation/{calculationIdB}";

            return await DeleteAsync(url);
        }

        public async Task<HttpStatusCode> DeleteCalculationSpecificationRelationship(string calculationId, string specificationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            
            string url = $"{UrlRoot}/specification/{specificationId}/relationships/calculation/{calculationId}";

            return await DeleteAsync(url);
        }

        public async Task<HttpStatusCode> UpsertCalculationDataFieldsRelationships(string calculationId, string[] dataFieldIds)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.ArgumentNotNull(dataFieldIds, nameof(dataFieldIds));

            string url = $"{UrlRoot}/calculation/{calculationId}/relationships/datafields";

            return await PostAsync(url, dataFieldIds);
        }
    }
}
