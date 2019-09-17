using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.DataSets;
using CalculateFunding.Common.ApiClient.DataSets.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;

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
            IEnumerable<DefinitionSpecificationRelationship> expectedResponse = new []
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
    }
}