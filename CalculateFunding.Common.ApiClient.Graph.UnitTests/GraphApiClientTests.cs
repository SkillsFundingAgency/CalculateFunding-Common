using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Graph.Models;
using CalculateFunding.Common.Testing;
using CalculateFunding.Common.Extensions;
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

        [TestMethod]
        public async Task UpsertDataset()
        {
            await AssertPostRequest("datasets",
                new Dataset(),
                HttpStatusCode.OK,
                _client.UpsertDataset);
        } 
        
        [TestMethod]
        public async Task DeleteDataset()
        {
            string id = NewRandomString();
            
            await AssertDeleteRequest($"datasets/{id}",
                id,
                HttpStatusCode.OK,
                _client.DeleteDataset);
        } 
        
        [TestMethod]
        public async Task UpsertDatasetDefinition()
        {
            await AssertPostRequest("datasetdefinitions",
                new DatasetDefinition(), 
                HttpStatusCode.OK,
                _client.UpsertDatasetDefinition);
        } 
        
        [TestMethod]
        public async Task DeleteDatasetDefinition()
        {
            string id = NewRandomString();
            
            await AssertDeleteRequest($"datasetdefinitions/{id}",
                id,
                HttpStatusCode.OK,
                _client.DeleteDatasetDefinition);
        } 
        
        [TestMethod]
        public async Task UpsertDatasetField()
        {
            await AssertPostRequest("datafields",
                new DataField(), 
                HttpStatusCode.OK,
                _client.UpsertDataFields);
        }

        [TestMethod]
        public async Task UpsertCalculationDatasetFieldsRelationships()
        {
            string id = NewRandomString();
            string[] ids = Strings(NewRandomString(), NewRandomString());

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            GivenTheStatusCode($"calculation/{id}/relationships/datasetfields",
                expectedStatusCode,
                HttpMethod.Post);

            HttpStatusCode apiResponse = await _client.UpsertCalculationDatasetFieldsRelationships(id, ids);

            apiResponse
                .Should()
                .Be(expectedStatusCode);

            AndTheRequestContentsShouldHaveBeen(ids.AsJson());
        }

        [TestMethod]
        public async Task UpsertCalculationDatasetFieldRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertPutRequest($"calculation/{idOne}/relationships/datasetfield/{idTwo}",
                HttpStatusCode.OK,
                () => _client.UpsertCalculationDatasetFieldRelationship(idOne, idTwo));
        }
        [TestMethod]
        public async Task DeleteDatasetField()
        {
            string id = NewRandomString();

            await AssertDeleteRequest($"datasetfield/{id}",
                id,
                HttpStatusCode.OK,
                _client.DeleteDatasetField);
        }

        [TestMethod]
        public async Task DeleteDataField()
        {
            string id = NewRandomString();
            
            await AssertDeleteRequest($"datafields/{id}",
                id,
                HttpStatusCode.OK,
                _client.DeleteDataField);
        }

        [TestMethod]
        public async Task UpsertDataDefinitionDatasetRelationship()
        {
            string definitionId = NewRandomString();
            string datasetId = NewRandomString();

            await AssertPutRequest($"datasetdefinitions/{definitionId}/relationships/datasets/{datasetId}",
                HttpStatusCode.OK,
                () => _client.UpsertDataDefinitionDatasetRelationship(definitionId, datasetId));
        }
        
        [TestMethod]
        public async Task DeleteDataDefinitionDatasetRelationship()
        {
            string definitionId = NewRandomString();
            string datasetId = NewRandomString();

            await AssertDeleteRequest($"datasetdefinitions/{definitionId}/relationships/datasets/{datasetId}",
                HttpStatusCode.OK,
                () => _client.DeleteDataDefinitionDatasetRelationship(definitionId, datasetId));
        }
        
        [TestMethod]
        public async Task UpsertDatasetDataFieldRelationship()
        {
            string datasetId = NewRandomString();
            string fieldId = NewRandomString();

            await AssertPutRequest($"datasets/{datasetId}/relationships/datafields/{fieldId}",
                HttpStatusCode.OK,
                () => _client.UpsertDatasetDataFieldRelationship(datasetId, fieldId));
        }
        
        [TestMethod]
        public async Task DeleteDatasetDataFieldRelationship()
        {
            string datasetId = NewRandomString();
            string fieldId = NewRandomString();

            await AssertDeleteRequest($"datasets/{datasetId}/relationships/datafields/{fieldId}",
                HttpStatusCode.OK,
                () => _client.DeleteDatasetDataFieldRelationship(datasetId, fieldId));
        }
        
        [TestMethod]
        public async Task CreateSpecificationDatasetRelationship()
        {
            string specificationId = NewRandomString();
            string datasetId = NewRandomString();

            await AssertPutRequest($"specifications/{specificationId}/relationships/datasets/{datasetId}",
                HttpStatusCode.OK,
                () => _client.CreateSpecificationDatasetRelationship(specificationId, datasetId));
        }
        
        [TestMethod]
        public async Task DeleteSpecificationDatasetRelationship()
        {
            string specificationId = NewRandomString();
            string datasetId = NewRandomString();

            await AssertDeleteRequest($"specifications/{specificationId}/relationships/datasets/{datasetId}",
                HttpStatusCode.OK,
                () => _client.DeleteSpecificationDatasetRelationship(specificationId, datasetId));
        }
        
        [TestMethod]
        public async Task CreateCalculationDataFieldRelationship()
        {
            string calculationId = NewRandomString();
            string fieldId = NewRandomString();

            await AssertPutRequest($"calculations/{calculationId}/relationships/datafields/{fieldId}",
                HttpStatusCode.OK,
                () => _client.CreateCalculationDataFieldRelationship(calculationId, fieldId));
        }
        
        [TestMethod]
        public async Task DeleteCalculationDataFieldRelationship()
        {
            string calculationId = NewRandomString();
            string fieldId = NewRandomString();

            await AssertDeleteRequest($"calculations/{calculationId}/relationships/datafields/{fieldId}",
                HttpStatusCode.OK,
                () => _client.DeleteCalculationDataFieldRelationship(calculationId, fieldId));
        }

        [TestMethod]
        public async Task UpsertDatasetFields()
        {
            await AssertPostRequest("datasetfields",
                DatasetFields(NewDatasetField(), NewDatasetField()),
                HttpStatusCode.OK,
                _client.UpsertDatasetFields);
        }

        private Calculation NewCalculation() => new Calculation();
        
        private Specification NewSpecification() => new Specification();

        private Calculation[] Calculations(params Calculation[] calculations) => calculations;

        private Specification[] Specifications(params Specification[] specifications) => specifications;

        private string[] Strings(params string[] ids) => ids;

        private DatasetField NewDatasetField() => new DatasetField();
        private DatasetField[] DatasetFields(params DatasetField[] datasetFields) => datasetFields;
    }
}