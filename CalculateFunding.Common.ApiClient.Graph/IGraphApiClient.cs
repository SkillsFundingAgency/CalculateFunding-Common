using CalculateFunding.Common.ApiClient.Graph.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Graph
{
    public interface IGraphApiClient
    {
        Task<HttpStatusCode> SaveCalculations(IList<Calculation> calculations);

        Task<HttpStatusCode> SaveSpecifications(IList<Specification> specifications);

        Task<HttpStatusCode> DeleteCalculation(string calculationId);

        Task<HttpStatusCode> DeleteSpecification(string specificationId);

        Task<HttpStatusCode> CreateCalculationCalculationsRelationships(string calculationId, string[] calculationIds);

        Task<HttpStatusCode> CreateCalculationCalculationRelationship(string calculationIdA, string calculationIdB);

        Task<HttpStatusCode> CreateCalculationSpecificationRelationship(string calculationId, string specificationId);

        Task<HttpStatusCode> DeleteCalculationCalculationRelationship(string calculationIdA, string calculationIdB);

        Task<HttpStatusCode> DeleteCalculationSpecificationRelationship(string calculationId, string specificationId);
    }
}
