using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Graph.Models;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;

// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Graph.UnitTests
{
    [TestClass]
    public class GraphApiClientTests : ApiClientTestBase
    {
        private GraphApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new GraphApiClient(ClientFactory,
                Logger.None);
        }

        [TestMethod]
        public async Task UpsertCalculations()
        {
            await AssertPostRequest("calculations",
                Calculations(NewCalculation(), NewCalculation()),
                HttpStatusCode.OK,
                _client.UpsertCalculations);
        }
        
        [TestMethod]
        public async Task UpsertSpecifications()
        {
            await AssertPostRequest("specifications",
                Specifications(NewSpecification(), NewSpecification(), NewSpecification()),
                HttpStatusCode.OK,
                _client.UpsertSpecifications);
        }

        [TestMethod]
        public async Task DeleteCalculation()
        {
            string id = NewRandomString();

            await AssertDeleteRequest($"calculation/{id}",
                id,
                HttpStatusCode.OK,
                _client.DeleteCalculation);
        }
        
        [TestMethod]
        public async Task DeleteSpecification()
        {
            string id = NewRandomString();

            await AssertDeleteRequest($"specification/{id}",
                id,
                HttpStatusCode.OK,
                _client.DeleteSpecification);
        }
        
        [TestMethod]
        public async Task DeleteAllForSpecification()
        {
            string id = NewRandomString();

            await AssertDeleteRequest($"specification/{id}/all",
                id,
                HttpStatusCode.OK,
                _client.DeleteAllForSpecification);
        }

        [TestMethod]
        public async Task UpsertCalculationCalculationsRelationships()
        {
            string id = NewRandomString();
            string[] ids = Strings(NewRandomString(), NewRandomString());

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            
            GivenTheStatusCode($"calculation/{id}/relationships/calculations",
                expectedStatusCode,
                HttpMethod.Post);

            HttpStatusCode apiResponse = await _client.UpsertCalculationCalculationsRelationships(id, ids);

            apiResponse
                .Should()
                .Be(expectedStatusCode);
            
            AndTheRequestContentsShouldHaveBeen(ids.AsJson());
        }

        [TestMethod]
        public async Task UpsertCalculationCalculationRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertPutRequest($"calculation/{idOne}/relationships/calculation/{idTwo}",
                HttpStatusCode.OK,
                () => _client.UpsertCalculationCalculationRelationship(idOne, idTwo));
        }

        [TestMethod]
        public async Task DeleteCalculationCalculationRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertDeleteRequest($"calculation/{idOne}/relationships/calculation/{idTwo}",
                HttpStatusCode.OK,
                () => _client.DeleteCalculationCalculationRelationship(idOne, idTwo));    
        }
        
        [TestMethod]
        public async Task DeleteCalculationSpecificationRelationship()
        {
            string specificationId = NewRandomString();
            string calculationId = NewRandomString();

            await AssertDeleteRequest($"specification/{specificationId}/relationships/calculation/{calculationId}",
                HttpStatusCode.OK,
                () => _client.DeleteCalculationSpecificationRelationship(calculationId, specificationId));    
        }
        
        private Calculation NewCalculation() => new Calculation();
        
        private Specification NewSpecification() => new Specification();

        private Calculation[] Calculations(params Calculation[] calculations) => calculations;

        private Specification[] Specifications(params Specification[] specifications) => specifications;

        private string[] Strings(params string[] ids) => ids;
    }
}