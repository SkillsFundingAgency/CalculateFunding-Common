using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver;
using CalculateFunding.Common.Graph.Interfaces;

namespace CalculateFunding.Common.Graph.UnitTests
{
    [TestClass]
    public class GraphRepositoryTests
    {
        private IGraphRepository _repository;
        private IDriver _driver;
        private IAsyncSession _session;
        private ICypherBuilderHost _cypherBuilderHost;
        private ICypherBuilder _cypherBuilder;

        [TestInitialize]
        public void SetUp()
        {
            _driver = Substitute.For<IDriver>();
            _session = Substitute.For<IAsyncSession>();
            _cypherBuilderHost = Substitute.For<ICypherBuilderHost>();
            _cypherBuilder = Substitute.For<ICypherBuilder>();

            _cypherBuilderHost
                .Current()
                .Returns(_cypherBuilder);

            _repository = new GraphRepository(new GraphDbSettingsBuilder().Build(), _cypherBuilderHost, _driver);
        }

        [TestMethod]
        public async Task AddNodes_GivenValidNodes_SuccessfullyAddsNodesToGraph()
        {
            GivenSessionOpened();
            await WhenAddNodes();
            ThenIndicesCreationIsCalled();
            ThenAddNodesCalled();
        }

        [TestMethod]
        public async Task DeleteNode_GivemExistingNode_SuccessfullyDeletesNodeFromGraph()
        {
            GivenSessionOpened();
            await WhenDeleteNode();
            ThenDeleteNodeCalled();
        }

        [TestMethod]
        public async Task CreateRelationship_GivenValidRelationship_SuccessfullyCreatesRelationshipInGraph()
        {
            GivenSessionOpened();
            await WhenCreateRelationship();
            ThenCreateRelationshipCalled();
        }

        private void ThenDeleteNodeCalled()
        {
            _cypherBuilder
                .Received(1)
                .AddMatch("(o:object{nodeid:'nodeid'})")
                .Received(1)
                .AddDetachDelete("o");
        }

        private void ThenAddNodesCalled()
        {
            _cypherBuilder
                .Received(1)
                .AddUnwind("{nodes} AS object")
                .Received(1)
                .AddMerge("o:object{nodeid:object.nodeid}")
                .Received(1)
                .AddSet("o = object");
        }

        private void ThenIndicesCreationIsCalled()
        {
            _cypherBuilder
                .Received(1)
                .AddCreate("INDEX ON :object(nodeid)");
        }

        private void ThenCreateRelationshipCalled()
        {
            _cypherBuilder
                .Received(1)
                .AddMatch("a: object),(b: object")
                .Received(1)
                .AddWhere("a.nodeid = 'node1' and b.nodeid = 'node2'")
                .Received(1)
                .AddCreate($"(a) -[:noderelation]->(b)");
        }

        private async Task WhenCreateRelationship()
        {
            await _repository.CreateRelationship<dynamic, dynamic>("noderelation", ("nodeid", "node1"), ("nodeid", "node2"));
        }

        private async Task WhenDeleteNode()
        {
            await _repository.DeleteNode<dynamic>("nodeid", "nodeid");
        }

        private async Task WhenAddNodes()
        {
            List<dynamic> nodes = new List<dynamic> { new { nodeid = "nodeid" } };
            await _repository.AddNodes(nodes, new string[] { "nodeid" });
        }

        private void GivenSessionOpened()
        {
            _driver
                .AsyncSession()
                .Returns(_session);
        }
    }
}
