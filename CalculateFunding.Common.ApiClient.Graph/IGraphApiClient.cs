using CalculateFunding.Common.ApiClient.Graph.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Graph
{
    public interface IGraphApiClient
    {
        Task<HttpStatusCode> SaveCalculations(IList<Calculation> calculations);
        Task<HttpStatusCode> DeleteCalculation(string calculationId);
    }
}
