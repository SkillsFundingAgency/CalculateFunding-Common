using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.Graph.Cosmos;
using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CalculateFunding.Common.Graph.UnitTests.Cosmos
{
    [TestClass]
    public class GraphRepositoryTests
    {
        private Mock<IGremlinClientFactory> _clientFactory;
        private Mock<IGremlinClient> _client;
        private Mock<IPathResultsTransform> _resultsTransform;

        private GraphRepository _repository;

        [TestInitialize]
        public void SetUp()
        {
            _clientFactory = new Mock<IGremlinClientFactory>();
            _client = new Mock<IGremlinClient>();
            _resultsTransform = new Mock<IPathResultsTransform>();

            _clientFactory.Setup(_ => _.CreateClient())
                .Returns(_client.Object);
            
            _repository = new GraphRepository(_clientFactory.Object,
                _resultsTransform.Object);
        }

        [TestMethod]
        public async Task GetCircularDependencies()
        {
            string relationship = NewRandomString();
            string name = NewRandomString();
            string value = NewRandomString();

            Field field = NewField(_ => _.WithName(name)
                .WithValue(value));
            
            string expectedQuery = $"g.V().hasLabel('model').has('{GremlinName(name)}', '{value}').as('A')" +
                                   $".repeat(bothE('{GremlinName(relationship)}').inV().simplePath()).emit(loops().is(gte(1)))" +
                                   $".bothE('{GremlinName(relationship)}').bothV().where(eq('A')).path()" +
                                   ".dedup().by(unfold().order().by(id).dedup().fold())";

            Dictionary<string, object>[] queryResponse = new Dictionary<string, object>[0];
            IEnumerable<Entity<Model>> expectedResults = new[]
            {
                NewModelEntity()
            };

            GivenTheResponseForTheQuery(queryResponse, expectedQuery);
            AndTheResultsAllTransformation("model", field, queryResponse, expectedResults);

            IEnumerable<Entity<Model>> actualResponse = await WhenTheCircularDependenciesAreQueried(relationship, field);

            actualResponse
                .Should()
                .BeEquivalentTo(expectedResults);
            
            AndTheClientWasDisposed();
        }

        [TestMethod]
        public async Task GetAllEntities()
        {
            string relationshipOne = NewRandomString();
            string relationshipTwo = NewRandomString();
            string name = NewRandomString();
            string value = NewRandomString();

            Field field = NewField(_ => _.WithName(name)
                .WithValue(value));
            
            string expectedQuery = $"g.V().hasLabel('model').has('{GremlinName(name)}', '{value}')" +
                                   $".coalesce(bothE('{GremlinName(relationshipOne)}','{GremlinName(relationshipTwo)}').otherV().path(), path())";

            Dictionary<string, object>[] queryResponse = new Dictionary<string, object>[0];
            IEnumerable<Entity<Model>> expectedResults = new[]
            {
                NewModelEntity()
            };

            GivenTheResponseForTheQuery(queryResponse, expectedQuery);
            AndTheResultsMatchesTransformation("model", field, queryResponse, expectedResults);

            IEnumerable<Entity<Model>> actualResponse = await WhenAllEntitiesAreQueried(field, relationshipOne, relationshipTwo);

            actualResponse
                .Should()
                .BeEquivalentTo(expectedResults);
            
            AndTheClientWasDisposed();
        }

        [TestMethod]
        public void UpsertNodesGuardsAgainstInvalidIndex()
        {
            string index = NewRandomString();
            
            Func<Task> invocation = () => WhenTheNodesAreUpserted(AsArray(index), NewModel());

            invocation
                .Should()
                .Throw<InvalidOperationException>()
                .Which
                .Message
                .Should()
                .Be($"Unable to upsert model as did not locate index property {index}");
        }

        [TestMethod]
        public async Task UpsertNodesWithNoPartitionKey()
        {
            string index = nameof(Model.AnIdProperty);
            
            Model modelOne = NewModel();
            Model modelTwo = NewModel();
            
            await WhenTheNodesAreUpserted(AsArray(index), modelOne, modelTwo);

            string expectedQueryForNodeOne = $"g.V().hasLabel('model').has('{GremlinName(index)}', '{modelOne.AnIdProperty}').fold()" +
                                             ".coalesce(unfold(), addV('model').property(single, 'partitionkey', 'FallBackPartitionKey'))" +
                                             $".property(single, 'anidproperty', '{modelOne.AnIdProperty}').property(single, 'name', '{modelOne.Name}')";
            string expectedQueryForNodeTwo = $"g.V().hasLabel('model').has('{GremlinName(index)}', '{modelTwo.AnIdProperty}').fold()" +
                                             ".coalesce(unfold(), addV('model').property(single, 'partitionkey', 'FallBackPartitionKey'))" +
                                             $".property(single, 'anidproperty', '{modelTwo.AnIdProperty}').property(single, 'name', '{modelTwo.Name}')";;
            
            ThenTheGremlinQueryWasExecuted(expectedQueryForNodeOne);
            AndTheGremlinQueryWasExecuted(expectedQueryForNodeTwo);
            AndTheClientWasDisposed(2);
        }
        
        [TestMethod]
        public async Task UpsertNodesWithPartitionKey()
        {
            string index = nameof(ModelWithImmutableProperties.Id);

            ModelWithImmutableProperties modelOne = NewModelWithImmutableProperties();
            ModelWithImmutableProperties modelTwo = NewModelWithImmutableProperties();
            
            await WhenTheNodesAreUpserted(AsArray(index), modelOne, modelTwo);

            string expectedQueryForNodeOne = $"g.V().hasLabel('modelwithimmutableproperties').has('{GremlinName(index)}', '{modelOne.Id}').fold()" +
                                             $".coalesce(unfold(), addV('modelwithimmutableproperties').property(single, 'id', '{modelOne.Id}').property(single, 'partitionkey', '{modelOne.PartitionKey}'))" +
                                             $".property(single, 'name', '{modelOne.Name}')";
            string expectedQueryForNodeTwo = $"g.V().hasLabel('modelwithimmutableproperties').has('{GremlinName(index)}', '{modelTwo.Id}').fold()" +
                                             $".coalesce(unfold(), addV('modelwithimmutableproperties').property(single, 'id', '{modelTwo.Id}').property(single, 'partitionkey', '{modelTwo.PartitionKey}'))" +
                                             $".property(single, 'name', '{modelTwo.Name}')";
            
            ThenTheGremlinQueryWasExecuted(expectedQueryForNodeOne);
            AndTheGremlinQueryWasExecuted(expectedQueryForNodeTwo);
            AndTheClientWasDisposed(2);
        }

        [TestMethod]
        public async Task DeleteNode()
        {
            Field field = NewField();

            await WhenTheNodeIsDeleted<Model>(field);

            string expectedQuery = $"g.V().hasLabel('model').has('{GremlinName(field.Name)}', '{field.Value}').drop()";
            
            ThenTheGremlinQueryWasExecuted(expectedQuery);
            AndTheClientWasDisposed();
        }

        [TestMethod]
        public async Task UpsertRelationship()
        {
            Field left = NewField();
            Field right = NewField();
            
            string relationship = NewRandomString();

            await WhenTheRelationshipIsUpserted<Model, ModelWithImmutableProperties>(relationship, left, right);

            string expectedQuery = $"g.V().hasLabel('model').has('{GremlinName(left.Name)}', '{left.Value}').as('A')" +
                                   $".V().hasLabel('modelwithimmutableproperties').has('{GremlinName(right.Name)}', '{right.Value}')" +
                                   $".coalesce(__.inE('{GremlinName(relationship)}').where(outV().as('A'))," +
                                   $"addE('{GremlinName(relationship)}').from('A'))";
            
            ThenTheGremlinQueryWasExecuted(expectedQuery);
            AndTheClientWasDisposed();
        }
        
        [TestMethod]
        public async Task DeleteRelationship()
        {
            Field left = NewField();
            Field right = NewField();
            
            string relationship = NewRandomString();

            await WhenTheRelationshipIsDeleted<Model, ModelWithImmutableProperties>(relationship, left, right);

            string expectedQuery =  $"g.V().hasLabel('model').has('{GremlinName(left.Name)}', '{left.Value}').bothE('{GremlinName(relationship)}')" +
                                   $".where(otherV().hasLabel('modelwithimmutableproperties').has('{GremlinName(right.Name)}', '{right.Value}')).drop()";

            
            ThenTheGremlinQueryWasExecuted(expectedQuery);
            AndTheClientWasDisposed();
        }
        
        private async Task WhenTheRelationshipIsDeleted<A, B>(string relationship,
            Field left,
            Field right)
        {
            await _repository.DeleteRelationship<A, B>(relationship, left, right);
        }

        private async Task WhenTheRelationshipIsUpserted<A, B>(string relationship,
            Field left,
            Field right)
        {
            await _repository.UpsertRelationship<A, B>(relationship, left, right);
        }

        private async Task WhenTheNodeIsDeleted<TNode>(Field field)
        {
            await _repository.DeleteNode<TNode>(field);
        }

        private async Task WhenTheNodesAreUpserted<TNode>(string[] indices,
            params TNode[] nodes)
        {
            await _repository.UpsertNodes(nodes, indices);
        }

        private string GremlinName(string name) => name.ToLowerInvariant();

        private TItem[] AsArray<TItem>(params TItem[] items) => items; 

        private async Task<IEnumerable<Entity<Model>>> WhenAllEntitiesAreQueried(Field field, 
            params string[] relationships) =>
            await _repository.GetAllEntities<Model>(field,
                relationships);

        private async Task<IEnumerable<Entity<Model>>> WhenTheCircularDependenciesAreQueried(string relationship,
            Field field) =>
            await _repository.GetCircularDependencies<Model>(relationship, field);

        private void ThenTheGremlinQueryWasExecuted(string expectedQuery)
        {
            _client.Verify(_ => _.SubmitAsync<Dictionary<string, object>>(
                    It.Is<RequestMessage>(rm => HasGremlinQuery(rm, expectedQuery))),
                Times.Once);
        }
        
        private void AndTheGremlinQueryWasExecuted(string expectedQuery)
        {
            ThenTheGremlinQueryWasExecuted(expectedQuery);
        }

        private void GivenTheResponseForTheQuery(Dictionary<string, object>[] response,
            string query)
        {
            _client.Setup(_ => _.SubmitAsync<Dictionary<string, object>>(It.Is<RequestMessage>(
                    rm => HasGremlinQuery(rm, query))))
                .ReturnsAsync(new ResultSet<Dictionary<string, object>>(response, new Dictionary<string, object>()));
        }

        private bool HasGremlinQuery(RequestMessage requestMessage,
            string query)
            => requestMessage.Arguments.TryGetValue(Tokens.ArgsGremlin, out object gremlinQuery) && 
               gremlinQuery?.Equals(query) == true;

        private void AndTheResultsAllTransformation(string vertexLabel,
            IField identifier,
            Dictionary<string, object>[] response,
            IEnumerable<Entity<Model>> transformedResponse)
        {
            _resultsTransform.Setup(_ => _.TransformAll<Model>(response, vertexLabel, identifier))
                .Returns(transformedResponse);
        }
        
        private void AndTheResultsMatchesTransformation(string vertexLabel,
            IField identifier,
            Dictionary<string, object>[] response,
            IEnumerable<Entity<Model>> transformedResponse)
        {
            _resultsTransform.Setup(_ => _.TransformMatches<Model>(response, vertexLabel, identifier))
                .Returns(transformedResponse);
        }

        private void AndTheClientWasDisposed(int times = 1)
        {
            _client.Verify(_ => _.Dispose(),
                Times.Exactly(times));
        }
        
        private string NewRandomString() => new RandomString();

        private Model NewModel(Action<ModelBuilder> setUp = null)
        {
            ModelBuilder modelBuilder = new ModelBuilder();

            setUp?.Invoke(modelBuilder);
            
            return modelBuilder.Build();
        }

        private Field NewField(Action<FieldBuilder> setUp = null)
        {
            FieldBuilder fieldBuilder = new FieldBuilder();

            setUp?.Invoke(fieldBuilder);
            
            return fieldBuilder.Build();
        }

        private Entity<Model> NewModelEntity(Action<EntityBuilder<Model>> setUp = null)
        {
            EntityBuilder<Model> entityBuilder = new EntityBuilder<Model>();

            setUp?.Invoke(entityBuilder);
            
            return entityBuilder.Build();
        }

        private ModelWithImmutableProperties NewModelWithImmutableProperties(Action<ModelWithImmutablePropertiesBuilder> setUp = null)
        {
            ModelWithImmutablePropertiesBuilder modelWithImmutablePropertiesBuilder = new ModelWithImmutablePropertiesBuilder();

            setUp?.Invoke(modelWithImmutablePropertiesBuilder);
            
            return modelWithImmutablePropertiesBuilder.Build();
        }
    }
}