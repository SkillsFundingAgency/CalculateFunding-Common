using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Policies.Models.ViewModels;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Policies.UnitTests
{
    [TestClass]
    public class PoliciesApiClientTests : ApiClientTestBase
    {
        private PoliciesApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new PoliciesApiClient(ClientFactory,
                Logger.None);
        }
        
        [TestMethod]
        public async Task GetFundingConfiguration()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();

            await AssertGetRequest($"configuration/{fundingStreamId}/{fundingPeriodId}",
                new FundingConfiguration(),
                () => _client.GetFundingConfiguration(fundingStreamId, fundingPeriodId));
        }
        
        [TestMethod]
        public async Task GetFundingConfigurationsByFundingStreamId()
        {
            string fundingStreamId = NewRandomString();

            await AssertGetRequest($"configuration/{fundingStreamId}",
                fundingStreamId,
                Enumerable.Empty<FundingConfiguration>(),
                _client.GetFundingConfigurationsByFundingStreamId);
        }
        
        [TestMethod]
        public async Task GetFundingPeriodById()
        {
            string fundingPeriodId = NewRandomString();

            await AssertGetRequest($"fundingperiods/{fundingPeriodId}",
                fundingPeriodId,
                new FundingPeriod(),
                _client.GetFundingPeriodById);
        }
        
        [TestMethod]
        public async Task GetFundingPeriods()
        {
            await AssertGetRequest($"fundingperiods",
                Enumerable.Empty<FundingPeriod>(),
                _client.GetFundingPeriods);
        }
        
        [TestMethod]
        public async Task GetFundingSchemaByVersion()
        {
            string schemaVersion = NewRandomString();
            
            await AssertGetRequest($"schemas/{schemaVersion}",
                schemaVersion,
                NewRandomString(),
                _client.GetFundingSchemaByVersion);
        }
        
        [TestMethod]
        public async Task GetFundingStreamById()
        {
            string fundingStreamId = NewRandomString();
            
            await AssertGetRequest($"fundingstreams/{fundingStreamId}",
                fundingStreamId,
                new FundingStream(),
                _client.GetFundingStreamById);
        }
        
        [TestMethod]
        public async Task GetFundingStreams()
        {
            await AssertGetRequest($"fundingstreams",
                Enumerable.Empty<FundingStream>(),
                _client.GetFundingStreams);
        }
        
        [TestMethod]
        public async Task GetFundingTemplate()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string templateVersion = NewRandomString();

            await AssertGetRequest($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}",
                new FundingTemplateContents(),
                () => _client.GetFundingTemplate(fundingStreamId, fundingPeriodId,templateVersion));
        }

        [TestMethod]
        public async Task SaveFundingConfiguration()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            FundingConfigurationUpdateViewModel updateViewModel = new FundingConfigurationUpdateViewModel();

            await AssertPostRequest($"configuration/{fundingStreamId}/{fundingPeriodId}",
                updateViewModel,
                new FundingConfiguration(),
                () => _client.SaveFundingConfiguration(fundingStreamId, fundingPeriodId, updateViewModel));
        }

        [TestMethod]
        public async Task SaveFundingPeriods()
        {
            await AssertPostRequest("fundingperiods",
                new FundingPeriodsUpdateModel(),
                new FundingPeriod(),
                _client.SaveFundingPeriods);
        }

        [TestMethod]
        [Ignore("This has a messed impl internally as it ignores the schema arg passed in")]
        public async Task SaveFundingSchema()
        {
            //TODO; either fix the impl or change the sig to remove the redundant parameter
            await AssertPostRequest("schemas",
                NewRandomString(),
                NewRandomString(),
                _client.SaveFundingSchema);
        }

        [TestMethod]
        public async Task SaveFundingStream()
        {
            await AssertPostRequest("fundingstreams",
                new FundingStreamUpdateModel(),
                new FundingStream(),
                _client.SaveFundingStream);
        }

        [TestMethod]
        [Ignore("This has a messed impl internally as it ignores the schema arg passed in")]
        public async Task SaveFundingTemplate()
        {
            await AssertPostRequest("schemas",
                NewRandomString(),
                NewRandomString(),
                _client.SaveFundingTemplate);   
        }

        [TestMethod]
        public async Task GetFundingTemplateSourceFile()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string templateVersion = NewRandomString();

            await AssertGetRequest($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/sourcefile",
                NewRandomString(),
                () => _client.GetFundingTemplateSourceFile(fundingStreamId, fundingPeriodId, templateVersion));

        }
        
        [TestMethod]
        public async Task GetFundingTemplateContents()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string templateVersion = NewRandomString();

            await AssertGetRequest($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata",
                new TemplateMetadataContents(),
                () => _client.GetFundingTemplateContents(fundingStreamId, fundingPeriodId,templateVersion));
        }
    }
}