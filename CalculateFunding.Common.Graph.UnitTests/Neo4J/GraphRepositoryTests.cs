using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Graph.Neo4J;
using CalculateFunding.Common.Graph.Serializer;
using CalculateFunding.Common.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4j.Driver;
using NSubstitute;

namespace CalculateFunding.Common.Graph.UnitTests.Neo4J
{
    [TestClass]
    public class GraphRepositoryTests
    {
        private IGraphRepository _repository;
        private IDriver _driver;
        private IAsyncSession _session;
        private IAsyncTransaction _transaction;

        [TestInitialize]
        public void SetUp()
        {
            _driver = Substitute.For<IDriver>();
            _session = Substitute.For<IAsyncSession>();
            _transaction = Substitute.For<IAsyncTransaction>();

            _repository = new GraphRepository(new GraphDbSettingsBuilder()
                    .Build(),
                new CypherBuilderFactory(),
                _driver);

            _driver
                .AsyncSession()
                .Returns(_session);

            _session
                .When(_ => _.WriteTransactionAsync(Arg.Any<Func<IAsyncTransaction, Task>>()))
                .Do(_ => _.ArgAt<Func<IAsyncTransaction, Task>>(0).Invoke(_transaction));

            _session
                .When(_ => _.ReadTransactionAsync(Arg.Any<Func<IAsyncTransaction, Task<List<IRecord>>>>()))
                .Do(_ => _.ArgAt<Func<IAsyncTransaction, Task<List<IRecord>>>>(0).Invoke(_transaction));
        }

        [TestMethod]
        public async Task AddNodes_GivenValidNodes_SuccessfullyAddsNodesToGraph()
        {
            string nodeId = NewRandomString();
            string index = NewRandomString();
            dynamic[] expectedNodes = { new { nodeId } };
            
            await WhenTheNodesAreAdded(expectedNodes, index);

            await ThenCypherWasExecutedInTheSession($"CREATE INDEX ON :object({index})\r\n");
            await AndTheCypherWasExecutedWithParameters("UNWIND {nodes} AS object\r\n" +
                                                                            $"MERGE(o:object{{{index}:object.{index}}})\r\n" +
                                                                            "SET o = object\r\n",
                ("nodes", expectedNodes));
            await AndTheSessionWasClosed();
        }

        [TestMethod]
        public async Task DeleteNode_GivenExistingNode_SuccessfullyDeletesNodeFromGraph()
        {
            string field = NewRandomString();
            string value = NewRandomString();

            await WhenTheNodeIsDeleted(field, value);

            await ThenTheCypherWasExecuted($"MATCH (o:object{{{field}:'{value}'}})\r\nDETACH DELETE o\r\n");
            await AndTheSessionWasClosed();
        }

        [TestMethod]
        public async Task CreateRelationship_GivenValidRelationship_SuccessfullyCreatesRelationshipInGraph()
        {
            string relationShipName = NewRandomString();
            string field = NewRandomString();
            string valueA = NewRandomString();
            string valueB = NewRandomString();

            await WhenTheRelationshipIsCreated(relationShipName, field, valueA, valueB);

            await ThenTheCypherWasExecuted("MATCH (a: object),(b: object)\r\n" + "" +
                                           $"WHERE a.{field} = '{valueA}' and b.{field} = '{valueB}'\r\n" +
                                           $"MERGE((a) -[:{relationShipName}]->(b))\r\n");

            await AndTheSessionWasClosed();
        }

        [TestMethod]
        public async Task DeleteRelationship_GivenValidRelationship_SuccessfullyDeleteRelationshipInGraph()
        {
            string relationShipName = NewRandomString();
            string field = NewRandomString();
            string valueA = NewRandomString();
            string valueB = NewRandomString();

            await WhenTheRelationshipIsDeleted(relationShipName, field, valueA, valueB);

            await ThenTheCypherWasExecuted($"MATCH (a: object)-[r:{relationShipName}]->(b: object)\r\n" + "" +
                                           $"WHERE a.{field} = '{valueA}' and b.{field} = '{valueB}'\r\n" +
                                           "DELETE r\r\n");
            await AndTheSessionWasClosed();
        }

        [TestMethod]
        public async Task GetCircularDependencies()
        {
            string field = NewRandomString();
            string value = NewRandomString();

            string query = "MATCH (e:object)\r\nWHERE SIZE((e)<-[:relatedto] - ()) <> 0\r\n"
                                        + "AND SIZE(()<-[:relatedto] - (e)) <> 0\r\n"
                                        + $"AND e.{field} = '{value}'\r\n"
                                        + "MATCH path=(e) <-[:relatedto *]-(e)\r\n"
                                        + "RETURN e, path\r\n";

            await WhenCircularDependenciesCalled(field, value);
            await ThenTheCypherWasExecuted(query);

            await AndTheSessionWasClosed();
        }

        [TestMethod]
        public async Task GetAllEntities()
        {
            string field = NewRandomString();
            string value = NewRandomString();

            string query = "MATCH (e:object)\r\n" +
                $"WHERE e.{field} = '{value}'\r\n" +
                "MATCH path=(e) <-[:connectto|:connectfrom *]-()\r\n" +
                "RETURN e, path\r\n";

            await WhenGetAllEntitiesCalled(field, value, new[] { "connectto", "connectfrom" });
            await ThenTheCypherWasExecuted(query);

            await AndTheSessionWasClosed();
        }

        private async Task ThenCypherWasExecutedInTheSession(string expectedCypher)
        {
            await _session
                .Received(1)
                .RunAsync(expectedCypher);
        }

        private async Task AndTheCypherWasExecutedWithParameters(string expectedQueryText, 
            params (string, object)[] expectedParameters)
        {
            await _transaction
                .Received(1)
                .RunAsync(expectedQueryText, Arg.Is<Dictionary<string, object>>(_ => 
                    ParametersMatch(_, expectedParameters)));
        }

        private bool ParametersMatch(Dictionary<string, object> actualParameters, (string, object)[] expectedParameters)
        {
            Dictionary<string, IEnumerable<Dictionary<string, object>>> expectedParameterDictionaries =
                expectedParameters.ToDictionary(_ => _.Item1, _ => ParameterSerializer.ToDictionaries((IEnumerable<dynamic>) _.Item2));

            foreach (KeyValuePair<string,object> actualParameter in actualParameters)
            {
                if (!expectedParameterDictionaries.TryGetValue(actualParameter.Key, out IEnumerable<Dictionary<string, object>> expectedValues))
                {
                    return false;
                }

                IEnumerable<Dictionary<string, object>> actualValues = actualParameter.Value as IEnumerable<Dictionary<string, object>>;

                if (actualValues?.Count() != expectedValues.Count())
                {
                    return false;
                }

                if (actualValues.Any(actualValue => 
                    !expectedValues.Contains(actualValue, new ParameterDictionaryComparer())))
                {
                    return false;
                }
            }
            
            return true;
        }

        private class ParameterDictionaryComparer : IEqualityComparer<Dictionary<string, object>>
        {
            public bool Equals(Dictionary<string, object> x, Dictionary<string, object> y)
            {
                return GetHashCode(x ?? new Dictionary<string, object>()) ==
                       GetHashCode(y ?? new Dictionary<string, object>());
            }

            public int GetHashCode(Dictionary<string, object> obj)
            {
                unchecked
                {
                    int hashCode = 0;

                    foreach (KeyValuePair<string,object> pair in obj)
                    {
                        hashCode = HashCode.Combine(hashCode, pair.Key.GetHashCode(), pair.Value?.GetHashCode());
                    }

                    return hashCode;
                }
            }
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

        private async Task WhenCircularDependenciesCalled(string field, string value)
        {
            await _repository.GetCircularDependencies<dynamic>("relatedto", new Field { Name = field, Value = value });
        }

        private async Task WhenGetAllEntitiesCalled(string field, string value, IEnumerable<string> relationships)
        {
            await _repository.GetAllEntities<dynamic>(new Field { Name = field, Value = value }, relationships);
        }

        private async Task WhenTheRelationshipIsCreated(string relationShipName, string field, string valueA, string valueB)
        {
            await _repository.UpsertRelationship<dynamic, dynamic>(relationShipName,
                (new Field { Name = field, Value = valueA }),
                (new Field { Name = field, Value = valueB }));
        }

        private async Task WhenTheRelationshipIsDeleted(string relationShipName, string field, string valueA, string valueB)
        {
            await _repository.DeleteRelationship<dynamic, dynamic>(relationShipName,
                (new Field { Name = field, Value = valueA }),
                (new Field { Name = field, Value = valueB }));
        }

        private async Task WhenTheNodeIsDeleted(string field, string value)
        {
            await _repository.DeleteNode<dynamic>(new Field { Name = field, Value = value });
        }

        private async Task WhenTheNodesAreAdded(IEnumerable<dynamic> nodes, params string[] indices)
        {
            await _repository.UpsertNodes(nodes, indices);
        }

        private string NewRandomString() =>  new RandomString();
    }
}