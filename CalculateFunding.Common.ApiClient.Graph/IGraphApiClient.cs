using CalculateFunding.Common.ApiClient.Graph.Models;
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
    }
}
