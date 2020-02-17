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

        public async Task<HttpStatusCode> UpsertCalculations(Calculation[] calculations)
        {
            Guard.ArgumentNotNull(calculations, nameof(calculations));

            string url = $"{UrlRoot}/calculations";

            return await PostAsync(url, calculations);
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
            //TODO; detach delete all calcs with specification id
            //TODO; detach delete spec with id

            return HttpStatusCode.NotImplemented;
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
