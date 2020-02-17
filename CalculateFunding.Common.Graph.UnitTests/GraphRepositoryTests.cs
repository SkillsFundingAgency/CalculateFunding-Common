using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver;
using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.Graph.UnitTests
{
    [TestClass]
    public class GraphRepositoryTests
    {
        private IGraphRepository _repository;
        private IDriver _driver;
        private IAsyncSession _session;
        private IAsyncTransaction _transaction;
        private ICypherBuilderFactory _cypherBuilderFactory;
        private ICypherBuilder _cypherBuilder;

        [TestInitialize]
        public void SetUp()
        {
            _driver = Substitute.For<IDriver>();
            _session = Substitute.For<IAsyncSession>();
            _cypherBuilderFactory = Substitute.For<ICypherBuilderFactory>();
            _cypherBuilder = Substitute.For<ICypherBuilder>();
            _transaction = Substitute.For<IAsyncTransaction>();

            _cypherBuilderFactory
                .NewCypherBuilder()
                .Returns(_cypherBuilder);

            _repository = new GraphRepository(new GraphDbSettingsBuilder().Build(), _cypherBuilderFactory, _driver);
            
            _driver
                .AsyncSession()
                .Returns(_session);
            
            _session
                .When(_ => _.WriteTransactionAsync(Arg.Any<Func<IAsyncTransaction, Task>>()))
                .Do(_ => _.ArgAt<Func<IAsyncTransaction, Task>>(0).Invoke(_transaction));
        }
        
        [TestMethod]
        public async Task AddNodes_GivenValidNodes_SuccessfullyAddsNodesToGraph()
        {
            await WhenAddNodes();
            ThenIndicesCreationIsCalled();
            ThenAddNodesCalled();
        }

        [TestMethod]
        public async Task DeleteNode_GivemExistingNode_SuccessfullyDeletesNodeFromGraph()
        {
            await WhenDeleteNode();
            ThenDeleteNodeCalled();
        }

        [TestMethod]
        public async Task CreateRelationship_GivenValidRelationship_SuccessfullyCreatesRelationshipInGraph()
        {
            await WhenCreateRelationship();
            ThenCreateRelationshipCalled();
        }

        [TestMethod]
        public async Task DeleteRelationship_GivenValidRelationship_SuccessfullyDeleteRelationshipInGraph()
        {
            await WhenDeleteRelationship();
            ThenDeleteRelationshipCalled();
        }

        [TestMethod]
        public async Task DeleteNodeAndChildNodesDetachDeletesRootAndImmediateChildrenInGraphFromRootWithSuppliedFieldValue()
        {
            GivenAConcreteCypherBuilder();
            
            string field = NewRandomString();
            string value = NewRandomString();

            await WhenTheNodeAndChildrenAreDeleted(field, value);

            await ThenTheCypherWasExecuted($"MATCH((o:object{{{field}:'{value}'}})-[*0..]->(x))\r\nDETACH DELETE x\r\n");
            await AndTheSessionWasClosed();
        }

        private void GivenAConcreteCypherBuilder()
        {
            _repository = new GraphRepository(new GraphDbSettingsBuilder()
                .Build(), 
                new CypherBuilderFactory(), 
                _driver);    
        }

        private async Task ThenTheCypherWasExecuted(string expectedCypher)
        {
            await _transaction
                .Received(1)
                .RunAsync(expectedCypher);
        }

        private async Task AndTheSessionWasClosed()
        {
            await _session
                .Received(1)
                .CloseAsync();
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

        private void ThenDeleteRelationshipCalled()
        {
            _cypherBuilder
                .Received(1)
                .AddMatch("a: object)-[r:noderelation]->(b: object")
                .Received(1)
                .AddWhere("a.nodeid = 'node1' and b.nodeid = 'node2'")
                .Received(1)
                .AddDelete($"r");
        }

        private async Task WhenTheNodeAndChildrenAreDeleted(string field, string value)
        {
            await _repository.DeleteNodeAndChildNodes<dynamic>(field, value);
        }
        
        private async Task WhenCreateRelationship()
        {
            await _repository.CreateRelationship<dynamic, dynamic>("noderelation", ("nodeid", "node1"), ("nodeid", "node2"));
        }

        private async Task WhenDeleteRelationship()
        {
            await _repository.DeleteRelationship<dynamic, dynamic>("noderelation", ("nodeid", "node1"), ("nodeid", "node2"));
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
        
        private string NewRandomString() => new RandomString();
    }
}
