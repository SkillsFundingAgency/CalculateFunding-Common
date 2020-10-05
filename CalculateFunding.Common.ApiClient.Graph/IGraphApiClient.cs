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
        Task<HttpStatusCode> UpsertCalculationDataFieldsRelationships(string calculationId, string[] dataFieldIds);
        Task<HttpStatusCode> DeleteCalculationCalculationRelationship(string calculationIdA, string calculationIdB);
        Task<HttpStatusCode> DeleteCalculationSpecificationRelationship(string calculationId, string specificationId);
        Task<HttpStatusCode> UpsertDataset(Dataset dataset);
        Task<HttpStatusCode> UpsertDatasets(Dataset[] datasets);
        Task<HttpStatusCode> DeleteDataset(string datasetId);
        Task<HttpStatusCode> UpsertDatasetDefinition(DatasetDefinition definition);
        Task<HttpStatusCode> UpsertDatasetDefinitions(DatasetDefinition[] definitions);
        Task<HttpStatusCode> UpsertDataField(DataField dataField);
        Task<HttpStatusCode> UpsertDataFields(DataField[] dataFields);
        Task<HttpStatusCode> DeleteDatasetDefinition(string definitionId);
        Task<HttpStatusCode> DeleteDataField(string dataFieldId);
        Task<HttpStatusCode> UpsertDataDefinitionDatasetRelationship(string definitionId, string datasetId);
        Task<HttpStatusCode> DeleteDataDefinitionDatasetRelationship(string definitionId, string datasetId);
        Task<HttpStatusCode> DeleteDatasetDataFieldRelationship(string datasetId, string fieldId);
        Task<HttpStatusCode> UpsertDatasetDataFieldRelationship(string datasetId, string fieldId);
        Task<HttpStatusCode> UpsertSpecificationDatasetRelationship(string specificationId, string datasetId);
        Task<HttpStatusCode> DeleteSpecificationDatasetRelationship(string specificationId, string datasetId);
        Task<HttpStatusCode> DeleteCalculationDataFieldRelationship(string calculationId, string fieldId);
        Task<HttpStatusCode> UpsertCalculationDataFieldRelationship(string calculationId, string fieldId);
        Task<HttpStatusCode> UpsertFundingLines(FundingLine[] fundingLines);
        Task<HttpStatusCode> DeleteFundingLine(string fieldId);
        Task<HttpStatusCode> UpsertFundingLineCalculationRelationship(string fundingLineId, string calculationId);
        Task<HttpStatusCode> UpsertCalculationFundingLineRelationship(string calculationId, string fundingLineId);
        Task<HttpStatusCode> DeleteFundingLineCalculationRelationship(string fundingLineId, string calculationId);
        Task<HttpStatusCode> DeleteCalculationFundingLineRelationship(string calculationId, string fundingLineId);
        Task<ApiResponse<IEnumerable<Entity<Calculation>>>> GetCircularDependencies(string specificationId);
        Task<ApiResponse<IEnumerable<Entity<Specification>>>> GetAllEntitiesRelatedToSpecification(string specificationId);
        Task<ApiResponse<IEnumerable<Entity<Calculation>>>> GetAllEntitiesRelatedToCalculation(string calculationId);
        Task<ApiResponse<IEnumerable<Entity<DataField>>>> GetAllEntitiesRelatedToDataset(string datasetFieldId);
        Task<ApiResponse<IEnumerable<Entity<FundingLine>>>> GetAllEntitiesRelatedToFundingLine(string fundingLineId);
    }
}
