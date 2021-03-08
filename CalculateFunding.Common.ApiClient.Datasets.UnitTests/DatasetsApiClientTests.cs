using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Datasets.Models;
using CalculateFunding.Common.ApiClient.DataSets;
using CalculateFunding.Common.ApiClient.DataSets.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Datasets.UnitTests
{
    [TestClass]
    public class DatasetsApiClientTests : ApiClientTestBase
    {
        private DatasetsApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new DatasetsApiClient(ClientFactory,
                Substitute.For<ILogger>());
        }

        [TestMethod]
        public async Task SaveDefinitionMakesPostCallWithYamlInBodyAndFileNameInCustomHeader()
        {
            string expectedYaml = NewRandomString();
            string expectedYamlFileName = NewRandomString();

            GivenTheStatusCode("datasets/data-definitions", HttpStatusCode.OK, HttpMethod.Post, "yaml-file", expectedYamlFileName);

            HttpStatusCode statusCode = await _client.SaveDefinition(expectedYaml, expectedYamlFileName);

            statusCode
                .Should()
                .Be(HttpStatusCode.OK);

            AndTheRequestContentsShouldHaveBeen(expectedYaml);
        }

        [TestMethod]
        [DataRow((string) null, "regenerate-providersourcedatasets")]
        [DataRow("spec1", "regenerate-providersourcedatasets?specificationId=spec1")]
        public async Task RegenerateProviderSourceDatasetsMakesPostCallWithSpecificationIdIfSupplied(string specificationId,
            string expectedCallUri)
        {
            IEnumerable<DefinitionSpecificationRelationship> expectedResponse = new[]
            {
                new DefinitionSpecificationRelationship(),
            };

            GivenTheResponse(expectedCallUri, expectedResponse, HttpMethod.Post);

            ApiResponse<IEnumerable<DefinitionSpecificationRelationship>> apiResponse = await _client.RegenerateProviderSourceDatasets(specificationId);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            apiResponse?.Content
                .Should()
                .BeEquivalentTo(expectedResponse);
        }

        [TestMethod]
        public async Task GetDatasetDefinitionsMakesGetCall()
        {
            await AssertGetRequest("get-data-definitions",
                Enumerable.Empty<DatasetDefinition>(),
                _client.GetDatasetDefinitions);
        }
        
        [TestMethod]
        public async Task GetDatasetDefinitionsByIdMakesGetCallWithSuppliedId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-dataset-definition-by-id?datasetDefinitionId={id}",
                id,
                new DatasetDefinition(),
                _ => _client.GetDatasetDefinitionById(_));
        }
        
        [TestMethod]
        public async Task GetDatasetDefinitionsByIdsMakesPostCallWithSuppliedIdsInBody()
        {
            await AssertPostRequest("get-dataset-definitions-by-ids",
                new string []
                {
                    NewRandomString(),
                    NewRandomString(),
                    NewRandomString(),
                },
                Enumerable.Empty<DatasetDefinition>(),
                _ => _client.GetDatasetDefinitionsByIds(_));
        }

        [TestMethod]
        public async Task CreateNewDatasetPostsSuppliedModel()
        {
            await AssertPostRequest("create-new-dataset",
                new CreateNewDatasetModel(),
                new NewDatasetVersionResponseModel(),
                _ => _client.CreateNewDataset(_));
        }
        
        [TestMethod]
        public async Task DatasetVersionUpdatePostsSuppliedModel()
        {
            await AssertPostRequest("dataset-version-update",
                new DatasetVersionUpdateModel(),
                new NewDatasetVersionResponseModel(),
                _ => _client.DatasetVersionUpdate(_));
        }
        
        [TestMethod]
        public async Task SearchDatasetVersionPostsSuppliedModel()
        {
            await AssertPostRequest("datasets-version-search",
                new SearchModel(),
                new SearchResults<DatasetVersionIndex>(),
                _ => _client.SearchDatasetVersion(_));
        }
        
        [TestMethod]
        public async Task SearchDatasetDefinitionsPostsSuppliedModel()
        {
            await AssertPostRequest("dataset-definitions-search",
                new SearchModel(),
                new SearchResults<DatasetDefinitionIndex>(),
                _ => _client.SearchDatasetDefinitions(_));
        }
        
        [TestMethod]
        public async Task ValidateDatasetPostsSuppliedModel()
        {
            await AssertPostRequest("validate-dataset",
                new GetDatasetBlobModel(),
                new DatasetValidationStatusModel(),
                _ => _client.ValidateDataset(_));
        }
        
        [TestMethod]
        public async Task CreateRelationshipPostsSuppliedModel()
        {
            await AssertPostRequest("create-definitionspecification-relationship",
                new CreateDefinitionSpecificationRelationshipModel(),
                new DefinitionSpecificationRelationship(),
                _ => _client.CreateRelationship(_));
        }

        [TestMethod]
        public async Task GetRelationshipsBySpecificationIdMakesGetCallWithSuppliedId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-definitions-relationships?specificationId={id}",
                id,
                Enumerable.Empty<DatasetSpecificationRelationshipViewModel>(),
                _ => _client.GetRelationshipsBySpecificationId(_));
        }
        
        [TestMethod]
        public async Task GetRelationshipsBySpecificationIdAndNameMakesGetCallWithSuppliedIdAndName()
        {
            string id = NewRandomString();
            string name = NewRandomString();

            await AssertGetRequest($"get-definition-relationship-by-specificationid-name?specificationId={id}&name={name}",
                new DefinitionSpecificationRelationship(),
                () => _client.GetRelationshipBySpecificationIdAndName(id, name));
        }

        [TestMethod]
        public async Task GetDatasetByDatasetIdMakesGetCallWithSuppliedId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-dataset-by-datasetid?datasetId={id}",
                id,
                new DatasetViewModel(),
                _ => _client.GetDatasetByDatasetId(_));
        }

        [TestMethod]
        public async Task GetDatasetsByDefinitionIdMakesGetCallWithSuppliedId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-datasets-by-definitionid?definitionId={id}",
                id,
                Enumerable.Empty<DatasetViewModel>(),
                _ => _client.GetDatasetsByDefinitionId(_));
        }

        [TestMethod]
        public async Task GetCurrentRelationshipsBySpecificationIdMakesGetCallWithSuppliedId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-relationships-by-specificationId?specificationId={id}",
                id,
                Enumerable.Empty<DatasetSpecificationRelationshipViewModel>(),
                _ => _client.GetCurrentRelationshipsBySpecificationId(_));  
        }
        
        [TestMethod]
        public async Task GetDataSourcesByRelationshipIdMakesGetCallWithSuppliedId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-datasources-by-relationshipid?relationshipId={id}",
                id,
                new SelectDatasourceModel(),
                _ => _client.GetDataSourcesByRelationshipId(_));  
        }
        
        [TestMethod]
        public async Task AssignDatasourceVersionToRelationshipMakesPostCallWithSuppliedModel()
        {
            await AssertPostRequest("assign-datasource-to-relationship",
                new AssignDatasourceModel(),
                new JobCreationResponse(),
                _ => _client.AssignDatasourceVersionToRelationship(_));
        }
        
        [TestMethod]
        [DataRow((string)null)]
        [DataRow("version")]
        public async Task DownloadDatasetFileMakesGetCallWithSuppliedId(string version)
        {
            string id = NewRandomString();

            await AssertGetRequest(version.IsNullOrEmpty() ? 
                    $"download-dataset-file?datasetId={id}" :
                    $"download-dataset-file?datasetId={id}&datasetVersion={version}",
                new DatasetDownloadModel(),
                () => _client.DownloadDatasetFile(id, version));  
        }

        [TestMethod]
        public async Task ReIndexMakesGetCall()
        {
            await AssertGetRequest("reindex",
                NewRandomString(),
                _client.Reindex);
        }
        
        [TestMethod]
        public async Task ReindexDatasetVersionsMakesGetCall()
        {
            await AssertGetRequest("datasetsversions/reindex",
                NewRandomString(),
                _client.ReindexDatasetVersions);
        }

        [TestMethod]
        public async Task GetCurrentDatasetVersionByDatasetIdMakesGetCallWithSuppliedId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-currentdatasetversion-by-datasetid?datasetId={id}",
                id,
                new DatasetVersionResponseViewModel(),
                _ => _client.GetCurrentDatasetVersionByDatasetId(_));
        }

        [TestMethod]
        public async Task GetDatasetSchemaSasUrlMakesPostRequestWithSuppliedModel()
        {
            await AssertPostRequest("get-schema-download-url",
                new DatasetSchemaSasUrlRequestModel(),
                new DatasetSchemaSasUrlResponseModel(),
                _ => _client.GetDatasetSchemaSasUrl(_));
        }

        [TestMethod]
        public async Task GetValidateDatasetValidationErrorSasUrl()
        {
            await AssertPostRequest("get-validate-dataset-error-url",
                new DatasetValidationErrorRequestModel(),
                new DatasetValidationErrorSasUrlResponseModel(),
                _ => _client.GetValidateDatasetValidationErrorSasUrl(_));
        }

        [TestMethod]
        public async Task GetValidateDatasetStatus()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-dataset-validate-status?operationId={id}",
                new DatasetValidationStatusModel(),
                () => _client.GetValidateDatasetStatus(id));
        }
        
        [TestMethod]
        public async Task GetDatasetAggregationsBySpecificationId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"{id}/datasetAggregations",
                Enumerable.Empty<DatasetAggregations>(),
                () => _client.GetDatasetAggregationsBySpecificationId(id));
        }

        [TestMethod]
        public async Task GetSpecificationIdsForRelationshipDefinitionId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"{id}/relationshipSpecificationIds",
                Enumerable.Empty<string>(),
                () => _client.GetSpecificationIdsForRelationshipDefinitionId(id));     
        }

        [TestMethod]
        public async Task GetCurrentRelationshipsBySpecificationIdAndDatasetDefinitionId()
        {
            string specificationId = NewRandomString();
            string datasetDefinitionId = NewRandomString();

            await AssertGetRequest($"{specificationId}/{datasetDefinitionId}/relationships",
                Enumerable.Empty<DatasetSpecificationRelationshipViewModel>(),
                () => _client.GetCurrentRelationshipsBySpecificationIdAndDatasetDefinitionId(specificationId, datasetDefinitionId));
        }
        
        [TestMethod]
        public async Task UploadDatasetFile()
        {
            string fileName = NewRandomString();
            DatasetMetadataViewModel model = new DatasetMetadataViewModel();
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            
            GivenTheStatusCode($"upload-dataset-file/{fileName}", expectedStatusCode, HttpMethod.Post);

            HttpStatusCode apiResponse = await _client.UploadDatasetFile(fileName, model);

            apiResponse
                .Should()
                .Be(expectedStatusCode);
            
            AndTheRequestContentsShouldHaveBeen(model.AsJson());
        }

        [TestMethod]
        public async Task GetDatasetSchemaRelationshipModelsForSpecificationId()
        {
            string specificationId = NewRandomString();
            

            await AssertGetRequest($"{specificationId}/schemaRelationshipFields",
                Enumerable.Empty<DatasetSchemaRelationshipModel>(),
                () => _client.GetDatasetSchemaRelationshipModelsForSpecificationId(specificationId));
        }

        [TestMethod]
        public async Task GetDatasetDefinitionsByFundingStreamIdMakesGetCallWithSuppliedFundingStreamId()
        {
            string fundingStreamId = NewRandomString();

            await AssertGetRequest($"get-data-definitions/{fundingStreamId}",
                fundingStreamId,
                 Enumerable.Empty<DatasetDefinationByFundingStream>(),
                _ => _client.GetDatasetDefinitionsByFundingStreamId(_));
        }

    }
}