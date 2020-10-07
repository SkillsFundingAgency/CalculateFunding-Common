using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Policies.Models.ViewModels;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualBasic;
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
        public async Task SaveFundingTemplate()
        {
            string fundingStreamId = "fsId";
            string fundingPeriodId = "fdId";
            string templateVersion = "1.0";

            string template = NewRandomString();

            await AssertPostRequest($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}", 
                template, 
                NewRandomString(), 
                () => _client.SaveFundingTemplate(template, fundingStreamId, fundingPeriodId, templateVersion));
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
        
        [TestMethod]
        public async Task GetFundingTemplateContentsWhenEtagSupplied()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string templateVersion = NewRandomString();
            string etag = NewRandomHeaderValue();

            await AssertGetRequest($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata",
                new TemplateMetadataContents(),
                () => _client.GetFundingTemplateContents(fundingStreamId, fundingPeriodId,templateVersion, etag),
                "If-None-Match", etag);
        }

        [TestMethod]
        public async Task GetFundingTemplates()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            

            await AssertGetRequest($"templates/{fundingStreamId}/{fundingPeriodId}",
                 Enumerable.Empty<PublishedFundingTemplate>(),
                () => _client.GetFundingTemplates(fundingStreamId, fundingPeriodId));
        }

        [TestMethod]
        public async Task GetFundingDate()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string fundingLineId = NewRandomString();

            await AssertGetRequest($"fundingdates/{fundingStreamId}/{fundingPeriodId}/{fundingLineId}",
                new FundingDate(),
                () => _client.GetFundingDate(fundingStreamId, fundingPeriodId, fundingLineId));
        }

        [TestMethod]
        public async Task SaveFundingDate()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();
            string fundingLineId = NewRandomString();
            FundingDateUpdateViewModel updateViewModel = new FundingDateUpdateViewModel();

            await AssertPostRequest($"fundingdates/{fundingStreamId}/{fundingPeriodId}/{fundingLineId}",
                updateViewModel,
                new FundingDate(),
                () => _client.SaveFundingDate(fundingStreamId, fundingPeriodId, fundingLineId, updateViewModel));
        }

        [TestMethod]
        public async Task GetDistinctTemplateMetadataContents()
        {
            string fundingPeriodId = NewRandomString();
            string templateVersion = NewRandomString();
            string fundingStreamId = NewRandomString();
            string etag = NewRandomHeaderValue();
            
            await AssertGetRequest($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata/distinct",
                 new TemplateMetadataDistinctContents(),
                () => _client.GetDistinctTemplateMetadataContents(fundingStreamId, fundingPeriodId, templateVersion));
        }

        [TestMethod]
        public async Task GetDistinctTemplateMetadataFundingLinesContents()
        {
            string fundingPeriodId = NewRandomString();
            string templateVersion = NewRandomString();
            string fundingStreamId = NewRandomString();
            string etag = NewRandomHeaderValue();

            await AssertGetRequest($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata/distinct/funding-lines",
                 new TemplateMetadataDistinctFundingLinesContents(),
                () => _client.GetDistinctTemplateMetadataFundingLinesContents(fundingStreamId, fundingPeriodId, templateVersion));
        }

        [TestMethod]
        public async Task GetDistinctTemplateMetadataCalculationsContents()
        {
            string fundingPeriodId = NewRandomString();
            string templateVersion = NewRandomString();
            string fundingStreamId = NewRandomString();
            string etag = NewRandomHeaderValue();

            await AssertGetRequest($"templates/{fundingStreamId}/{fundingPeriodId}/{templateVersion}/metadata/distinct/calculations",
                 new TemplateMetadataDistinctCalculationsContents(),
                () => _client.GetDistinctTemplateMetadataCalculationsContents(fundingStreamId, fundingPeriodId, templateVersion));
        }
    }
}