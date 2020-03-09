using CalculateFunding.Common.ApiClient.Graph.Models;
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

        public async Task<HttpStatusCode> UpsertDataset(Dataset dataset)
        {
            Guard.ArgumentNotNull(dataset, nameof(dataset));

            return await PostAsync($"{UrlRoot}/datasets", dataset);
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

            return await PostAsync($"{UrlRoot}/datasetdefinitions",
                definition);
        }

        public async Task<HttpStatusCode> DeleteDatasetDefinition(string definitionId)
        {
            Guard.IsNullOrWhiteSpace(definitionId, nameof(definitionId));
            
            return await DeleteAsync($"{UrlRoot}/datasetdefinitions/{definitionId}");
        }
        
        public async Task<HttpStatusCode> UpsertDataFields(DataField field)
        {
            Guard.ArgumentNotNull(field, nameof(field));

            return await PostAsync($"{UrlRoot}/datafields",
                field);
        }
        
        public async Task<HttpStatusCode> DeleteDataField(string fieldId)
        {
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));
            
            return await DeleteAsync($"{UrlRoot}/datafields/{fieldId}");
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
        
        public async Task<HttpStatusCode> CreateSpecificationDatasetRelationship(string specificationId, string datasetId)
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
        
        public async Task<HttpStatusCode> CreateCalculationDataFieldRelationship(string calculationId, string fieldId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));

            return await PutAsync($"calculations/{calculationId}/relationships/datafields/{fieldId}");
        }
        
        public async Task<HttpStatusCode> DeleteCalculationDataFieldRelationship(string calculationId, string fieldId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(fieldId, nameof(fieldId));

            return await DeleteAsync($"calculations/{calculationId}/relationships/datafields/{fieldId}");
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

        public async Task<HttpStatusCode> DeleteAllForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await DeleteAsync($"{UrlRoot}/specification/{specificationId}/all");
        }

        public async Task<HttpStatusCode> UpsertCalculationCalculationsRelationships(string calculationId, string[] calculationIds)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.ArgumentNotNull(calculationIds, nameof(calculationIds));

            string url = $"{UrlRoot}/calculation/{calculationId}/relationships/calculations";

            return await PostAsync(url, calculationIds);
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
    }
}
