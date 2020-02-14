using CalculateFunding.Common.ApiClient.Graph.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;
using System;
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

        public async Task<HttpStatusCode> SaveCalculations(IList<Calculation> calculations)
        {
            Guard.ArgumentNotNull(calculations, nameof(calculations));

            string url = $"{UrlRoot}/calculations";

            return await PostAsync(url, calculations);
        }

        public async Task<HttpStatusCode> SaveSpecifications(IList<Specification> specifications)
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

        public async Task<HttpStatusCode> CreateCalculationCalculationsRelationships(string calculationId, string[] calculationIds)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.ArgumentNotNull(calculationIds, nameof(calculationIds));

            string url = $"{UrlRoot}/calculation/calculations/{calculationId}";

            return await PostAsync(url, calculationIds);
        }

        public async Task<HttpStatusCode> CreateCalculationCalculationRelationship(string calculationIdA, string calculationIdB)
        {
            Guard.IsNullOrWhiteSpace(calculationIdA, nameof(calculationIdA));
            Guard.IsNullOrWhiteSpace(calculationIdB, nameof(calculationIdB));

            string url = $"{UrlRoot}/calculation/calculation/{calculationIdA}/{calculationIdB}";

            return await PutAsync(url);
        }

        public async Task<HttpStatusCode> CreateCalculationSpecificationRelationship(string calculationId, string specificationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/calculation/specification/{calculationId}/{specificationId}";

            return await PutAsync(url);
        }

        public async Task<HttpStatusCode> DeleteCalculationCalculationRelationship(string calculationIdA, string calculationIdB)
        {
            Guard.IsNullOrWhiteSpace(calculationIdA, nameof(calculationIdA));
            Guard.IsNullOrWhiteSpace(calculationIdB, nameof(calculationIdB));

            string url = $"{UrlRoot}/delete/calculation/calculation/{calculationIdA}/{calculationIdB}";

            return await DeleteAsync(url);
        }

        public async Task<HttpStatusCode> DeleteCalculationSpecificationRelationship(string calculationId, string specificationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/delete/calculation/specification/{calculationId}/{specificationId}";

            return await DeleteAsync(url);
        }
    }
}
