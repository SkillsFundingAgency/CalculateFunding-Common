using System.Linq;
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
        public async Task UpsertFundingLines()
        {
            await AssertPostRequest("fundinglines",
                FundingLines(NewFundingLine(), NewFundingLine()),
                HttpStatusCode.OK,
                _client.UpsertFundingLines);
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
        public async Task DeleteFundingLine()
        {
            string id = NewRandomString();

            await AssertDeleteRequest($"fundingline/{id}",
                id,
                HttpStatusCode.OK,
                _client.DeleteFundingLine);
        }

        [TestMethod]
        public async Task DeleteEnum()
        {
            string id = NewRandomString();

            await AssertDeleteRequest($"enum/{id}",
                id,
                HttpStatusCode.OK,
                _client.DeleteEnum);
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
        public async Task UpsertCalculationFundingLineRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertPutRequest($"calculation/{idOne}/relationships/fundingline/{idTwo}",
                HttpStatusCode.OK,
                () => _client.UpsertCalculationFundingLineRelationship(idOne, idTwo));
        }

        [TestMethod]
        public async Task UpsertFundingLineCalculationRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertPutRequest($"fundingline/{idOne}/relationships/calculation/{idTwo}",
                HttpStatusCode.OK,
                () => _client.UpsertFundingLineCalculationRelationship(idOne, idTwo));
        }

        [TestMethod]
        public async Task UpsertCalculationSpecificationRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertPutRequest($"specification/{idTwo}/relationships/calculation/{idOne}",
                HttpStatusCode.OK,
                () => _client.UpsertCalculationSpecificationRelationship(idOne, idTwo));
        }

        [TestMethod]
        public async Task UpsertCalculationEnumRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertPutRequest($"calculation/{idOne}/relationships/enum/{idTwo}",
                HttpStatusCode.OK,
                () => _client.UpsertCalculationEnumRelationship(idOne, idTwo));
        }

        [TestMethod]
        public async Task UpsertEnumCalculationRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertPutRequest($"enum/{idOne}/relationships/calculation/{idTwo}",
                HttpStatusCode.OK,
                () => _client.UpsertEnumCalculationRelationship(idOne, idTwo));
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
        public async Task DeleteCalculationFundingLineRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertDeleteRequest($"calculation/{idOne}/relationships/fundingline/{idTwo}",
                HttpStatusCode.OK,
                () => _client.DeleteCalculationFundingLineRelationship(idOne, idTwo));
        }

        [TestMethod]
        public async Task DeleteFundingLineCalculationRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertDeleteRequest($"fundingline/{idOne}/relationships/calculation/{idTwo}",
                HttpStatusCode.OK,
                () => _client.DeleteFundingLineCalculationRelationship(idOne, idTwo));
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
            await AssertPostRequest("dataset",
                new Dataset(),
                HttpStatusCode.OK,
                _client.UpsertDataset);
        }

        [TestMethod]
        public async Task UpsertDatasets()
        {
            await AssertPostRequest("datasets",
                new[] { new Dataset(), new Dataset() },
                HttpStatusCode.OK,
                _client.UpsertDatasets);
        }

        [TestMethod]
        public async Task UpsertEnums()
        {
            await AssertPostRequest("enums",
                new[] { new Enum(), new Enum() },
                HttpStatusCode.OK,
                _client.UpsertEnums);
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
            await AssertPostRequest("datasetdefinition",
                new DatasetDefinition(), 
                HttpStatusCode.OK,
                _client.UpsertDatasetDefinition);
        }

        [TestMethod]
        public async Task UpsertDatasetDefinitions()
        {
            await AssertPostRequest("datasetdefinitions",
                new[] { new DatasetDefinition(), new DatasetDefinition() },
                HttpStatusCode.OK,
                _client.UpsertDatasetDefinitions);
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
            await AssertPostRequest("datafield",
                new DataField(), 
                HttpStatusCode.OK,
                _client.UpsertDataField);
        }

        [TestMethod]
        public async Task UpsertCalculationDataFieldsRelationships()
        {
            string id = NewRandomString();
            string[] ids = Strings(NewRandomString(), NewRandomString());

            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;

            GivenTheStatusCode($"calculation/{id}/relationships/datafields",
                expectedStatusCode,
                HttpMethod.Post);

            HttpStatusCode apiResponse = await _client.UpsertCalculationDataFieldsRelationships(id, ids);

            apiResponse
                .Should()
                .Be(expectedStatusCode);

            AndTheRequestContentsShouldHaveBeen(ids.AsJson());
        }

        [TestMethod]
        public async Task UpsertCalculationDataFieldRelationship()
        {
            string idOne = NewRandomString();
            string idTwo = NewRandomString();

            await AssertPutRequest($"calculations/{idOne}/relationships/datafields/{idTwo}",
                HttpStatusCode.OK,
                () => _client.UpsertCalculationDataFieldRelationship(idOne, idTwo));
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
                () => _client.UpsertSpecificationDatasetRelationship(specificationId, datasetId));
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
                () => _client.UpsertCalculationDataFieldRelationship(calculationId, fieldId));
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
            await AssertPostRequest("datafields",
                DatasetFields(NewDatasetField(), NewDatasetField()),
                HttpStatusCode.OK,
                _client.UpsertDataFields);
        }
        
        [TestMethod]
        public async Task DeleteCalculationSpecificationRelationships()
        {
            await AssertPostRequest("/specification/relationships/calculation/delete", 
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.DeleteCalculationSpecificationRelationships);
        }

        [TestMethod]
        public async Task UpsertSpecificationDatasetRelationships()
        {
            await AssertPostRequest("specifications/relationships/datasets", 
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.UpsertSpecificationDatasetRelationships);   
        }

        [TestMethod]
        public async Task DeleteSpecificationDatasetRelationships()
        {
            await AssertPostRequest("specifications/relationships/datasets/delete", 
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.DeleteSpecificationDatasetRelationships);      
        }

        [TestMethod]
        public async Task DeleteCalculationDataFieldRelationships()
        {
            await AssertPostRequest("calculations/relationships/datafields/delete", 
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.DeleteCalculationDataFieldRelationships);         
        }

        [TestMethod]
        public async Task UpsertCalculationDataFieldRelationships()
        {
            AmendRelationshipRequestModel[] requestModels = NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                NewAmendRelationshipRequestModel());
            
            await AssertPutRequest("calculations/relationships/datafields", 
                requestModels,
                HttpStatusCode.OK,
                () => _client.UpsertCalculationDataFieldRelationships(requestModels));
        }
        
        [TestMethod]
        public async Task DeleteCalculations()
        {
            await AssertPostRequest("calculation/delete", 
                Strings(NewRandomString(), NewRandomString()),
                HttpStatusCode.OK,
                _client.DeleteCalculations);         
        }

        [TestMethod]
        public async Task UpsertFundingLineCalculationRelationships()
        {
            await AssertPostRequest("fundingline/relationships/calculation",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.UpsertFundingLineCalculationRelationships);
        }
        
        [TestMethod]
        public async Task UpsertCalculationFundingLineRelationships()
        {
            await AssertPostRequest("calculation/relationships/fundingline",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.UpsertCalculationFundingLineRelationships);
        }
        
        [TestMethod]
        public async Task DeleteFundingLineCalculationRelationships()
        {
            await AssertPostRequest("fundingline/relationships/calculation/delete",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.DeleteFundingLineCalculationRelationships);
        }
        
        [TestMethod]
        public async Task DeleteCalculationFundingLineRelationships()
        {
            await AssertPostRequest("calculation/relationships/fundingline/delete",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.DeleteCalculationFundingLineRelationships);
        }

        [TestMethod]
        public async Task DeleteCalculationCalculationRelationships()
        {
            await AssertPostRequest(string.Empty,
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.DeleteCalculationCalculationRelationships);
        }

        [TestMethod]
        public async Task UpsertDatasetDataFieldRelationships()
        {
            await AssertPostRequest("datasets/relationships/datafields",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.UpsertDatasetDataFieldRelationships);
        }
        
        [TestMethod]
        public async Task UpsertDataDefinitionDatasetRelationships()
        {
            await AssertPostRequest("datasetdefinitions/relationships/datasets",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.UpsertDataDefinitionDatasetRelationships);
        }
        
        [TestMethod]
        public async Task UpsertCalculationEnumRelationships()
        {
            await AssertPostRequest("calculation/relationships/enum",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.UpsertCalculationEnumRelationships);
        }

        [TestMethod]
        public async Task UpsertEnumCalculationRelationships()
        {
            await AssertPostRequest("enum/relationships/calculation",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.UpsertEnumCalculationRelationships);
        }

        [TestMethod]
        public async Task UpsertCalculationSpecificationRelationships()
        {
            await AssertPostRequest("specification/relationships/calculation",
                NewAmendRelationshipRequestModels(NewAmendRelationshipRequestModel(),
                    NewAmendRelationshipRequestModel()),
                HttpStatusCode.OK,
                _client.UpsertCalculationSpecificationRelationships);
        }

        [TestMethod]
        public async Task GetAllEntitiesRelatedToEnum()
        {
            string enumId = NewRandomString();

            await AssertGetRequest($"/enum/getallentities/{enumId}",
                new[]
                {
                    new Entity<Enum>
                    {
                        Node = NewEnum()
                    }
                }.AsEnumerable(),
                () => _client.GetAllEntitiesRelatedToEnum(enumId));
        }

        [TestMethod]
        public async Task GetAllEntitiesRelatedToCalculation()
        {
            string calculationId = NewRandomString();

            await AssertGetRequest($"/calculation/getallentities/{calculationId}",
                new[]
                {
                    new Entity<Calculation>
                    {
                        Node = NewCalculation()
                    }
                }.AsEnumerable(),
                () => _client.GetAllEntitiesRelatedToCalculation(calculationId));
        }

        [TestMethod]
        public async Task GetAllEntitiesRelatedToFundingLine()
        {
            string fundingLineId = NewRandomString();

            await AssertGetRequest($"/fundingline/getallentities/{fundingLineId}",
                new[]
                {
                    new Entity<FundingLine>
                    {
                        Node = NewFundingLine()
                    }
                }.AsEnumerable(),
                () => _client.GetAllEntitiesRelatedToFundingLine(fundingLineId));
        }

        [TestMethod]
        public async Task GetAllEntitiesRelatedToDataset()
        {
            string datafieldId = NewRandomString();

            await AssertGetRequest($"/dataset/getallentities/{datafieldId}",
                new[]
                {
                    new Entity<DataField>
                    {
                        Node = NewDataField()
                    }
                }.AsEnumerable(),
                () => _client.GetAllEntitiesRelatedToDataset(datafieldId));
        }

        [TestMethod]
        public async Task GetAllEntitiesRelatedToSpecification()
        {
            string specificationId = NewRandomString();

            await AssertGetRequest($"/specification/getallentities/{specificationId}",
                new[]
                {
                    new Entity<Specification>
                    {
                        Node = NewSpecification()
                    }
                }.AsEnumerable(),
                () => _client.GetAllEntitiesRelatedToSpecification(specificationId));
        }

        [TestMethod]
        public async Task GetCircularDependencies()
        {
            string specificationId = NewRandomString();

            await AssertGetRequest($"/calculation/circulardependencies/{specificationId}",
                new[]
                {
                    new Entity<Calculation>
                    {
                        Node = NewCalculation()
                    }
                }.AsEnumerable(),
                () => _client.GetCircularDependencies(specificationId));
        }

        [TestMethod]
        public async Task GetAllEntitiesRelatedToCalculations()
        {
            await AssertPostRequest("calculation/getallentitiesforall",
                Strings(NewRandomString(), NewRandomString()),
                AsEntities(NewCalculation(), NewCalculation())
                    .AsEnumerable(),
                _client.GetAllEntitiesRelatedToCalculations);
        }

        [TestMethod]
        public async Task GetAllEntitiesRelatedToFundingLines()
        {
            await AssertPostRequest("fundingline/getallentitiesforall",
                Strings(NewRandomString(), NewRandomString()),
                AsEntities(NewFundingLine(), NewFundingLine())
                    .AsEnumerable(),
                _client.GetAllEntitiesRelatedToFundingLines);
        }

        [TestMethod]
        public async Task DeleteFundingLines()
        {
            await AssertPostRequest("fundingline/delete",
                Strings(NewRandomString(), NewRandomString()),
                HttpStatusCode.OK,
                _client.DeleteFundingLines);    
        }

        [TestMethod]
        public async Task DeleteEnums()
        {
            await AssertPostRequest("enum/delete",
                Strings(NewRandomString(), NewRandomString()),
                HttpStatusCode.OK,
                _client.DeleteEnums);
        }

        private AmendRelationshipRequestModel NewAmendRelationshipRequestModel() => new AmendRelationshipRequestModel
        {
            IdA = NewRandomString()
        };

        private AmendRelationshipRequestModel[] NewAmendRelationshipRequestModels(params AmendRelationshipRequestModel[] requests) => requests;

        private Entity<TItem>[] AsEntities<TItem>(params TItem[] items)
            where TItem : class
            => items.Select(_ => new Entity<TItem>
            {
                Node = _
            }).ToArray();

        private Calculation NewCalculation() => new Calculation
        {
            CalculationId = NewRandomString()
        };

        private Enum NewEnum() => new Enum
        {
            EnumName = NewRandomString(),
            EnumValue = NewRandomString()
        };

        private FundingLine NewFundingLine() => new FundingLine
        {
            FundingLineId = NewRandomString()
        };

        private Dataset NewDataset() => new Dataset
        {
            DatasetId = NewRandomString()
        };

        private DataField NewDataField() => new DataField
        {
            DataFieldId = NewRandomString()
        };

        private Specification NewSpecification() => new Specification();

        private Calculation[] Calculations(params Calculation[] calculations) => calculations;

        private Specification[] Specifications(params Specification[] specifications) => specifications;

        private FundingLine[] FundingLines(params FundingLine[] fundingLines) => fundingLines;

        private string[] Strings(params string[] ids) => ids;

        private DataField NewDatasetField() => new DataField();
        private DataField[] DatasetFields(params DataField[] datasetFields) => datasetFields;
    }
}