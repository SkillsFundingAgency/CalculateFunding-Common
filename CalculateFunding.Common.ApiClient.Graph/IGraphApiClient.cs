using CalculateFunding.Common.ApiClient.Graph.Models;
using CalculateFunding.Common.ApiClient.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Graph
{
    public interface IGraphApiClient
    {
        Task<HttpStatusCode> UpsertCalculations(Calculation[] calculations);

        Task<HttpStatusCode> UpsertSpecifications(Specification[] specifications);

        Task<HttpStatusCode> DeleteCalculation(string calculationId);

        Task<HttpStatusCode> DeleteSpecification(string specificationId);

        Task<HttpStatusCode> UpsertCalculationCalculationsRelationships(string calculationId, string[] calculationIds);

        Task<HttpStatusCode> UpsertCalculationCalculationRelationship(string calculationIdA, string calculationIdB);

        Task<HttpStatusCode> UpsertCalculationSpecificationRelationship(string calculationId, string specificationId);

        Task<HttpStatusCode> DeleteCalculationCalculationRelationship(string calculationIdA, string calculationIdB);

        Task<HttpStatusCode> DeleteCalculationSpecificationRelationship(string calculationId, string specificationId);
        Task<HttpStatusCode> DeleteAllForSpecification(string specificationId);
        Task<HttpStatusCode> UpsertDataset(Dataset dataset);
        Task<HttpStatusCode> DeleteDataset(string datasetId);
        Task<HttpStatusCode> UpsertDatasetDefinition(DatasetDefinition definition);
        Task<HttpStatusCode> DeleteDatasetDefinition(string definitionId);
        Task<HttpStatusCode> UpsertDataFields(DataField field);
        Task<HttpStatusCode> UpsertDataDefinitionDatasetRelationship(string definitionId, string datasetId);
        Task<HttpStatusCode> DeleteDataDefinitionDatasetRelationship(string definitionId, string datasetId);
        Task<HttpStatusCode> DeleteDatasetDataFieldRelationship(string datasetId, string fieldId);
        Task<HttpStatusCode> CreateSpecificationDatasetRelationship(string specificationId, string datasetId);
        Task<HttpStatusCode> DeleteSpecificationDatasetRelationship(string specificationId, string datasetId);
        Task<HttpStatusCode> DeleteCalculationDataFieldRelationship(string calculationId, string fieldId);
        Task<HttpStatusCode> CreateCalculationDataFieldRelationship(string calculationId, string fieldId);

        Task<ApiResponse<IEnumerable<Entity<Calculation, JObject>>>> GetCircularDependencies(string specificationId);
    }
}
