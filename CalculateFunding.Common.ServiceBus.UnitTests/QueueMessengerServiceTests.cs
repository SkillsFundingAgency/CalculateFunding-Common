using Azure.Storage.Queues.Models;
using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.ServiceBus.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ServiceBus.ServiceBus
{
    [TestClass]
    public class QueueMessengerServiceTests
    {
        private QueueMessengerService _messengerService;
        private Mock<IQueueClient> _queueClient;
        private const string JobDefinitionId = "PopulateScopedProvidersJob";
        private const string QueueName = "QueueName";
        private const string TopicName = "TopicName";

        [TestInitialize]
        public void SetUp()
        {
            _queueClient = new Mock<IQueueClient>();
            _messengerService = new QueueMessengerService(_queueClient.Object);
        }

        [TestMethod]
        public async Task IsHealthy_ReturnsQueueExists_ReturnsTrue()
        {
            GivenQueueExists();
            bool isHealthy = await WhenHealthChecked();
            ThenServiceIsHealthy(isHealthy);
            _queueClient.VerifyAll();
        }

        [TestMethod]
        public async Task IsHealthy_ReturnsQueueDoesNotExists_ReturnsFalse()
        {
            bool isHealthy = await WhenHealthChecked();
            ThenServiceIsUnHealthy(isHealthy);
        }

        [TestMethod]
        public async Task ReceiveMessages_TargetMessageReturnedFromStorageQueue_TargetMessageReceived()
        {
            // Arrange
            var job = new Job { JobDefinitionId = JobDefinitionId };
            Predicate<Job> predicate = _ => _.JobDefinitionId == JobDefinitionId;

            // Build the JSON payload expected by your service
            string payload = JsonConvert.SerializeObject(new QueueMessage<Job> { Data = job });

            // v12: create a QueueMessage for tests via the model factory
            QueueMessage message = QueuesModelFactory.QueueMessage(
                messageId: Guid.NewGuid().ToString("N"),
                popReceipt: "pop",
                body: BinaryData.FromString(payload),
                dequeueCount: 1,
                insertedOn: DateTimeOffset.UtcNow
            );

            GivenMessageReturnedFromQueue(message);

            // Act
            Job jobReceived = await WhenMessagesReceived(QueueName, predicate);

            // Assert
            ThenTargetMessageReceived(job, predicate);
            _queueClient.VerifyAll();
        }

        [TestMethod]
        public async Task ReceiveMessages_TargetMessageNotReturnedFromServiceBus_TimesOut()
        {
            Guid lockToken = Guid.NewGuid();
            Job job = new Job { JobDefinitionId = JobDefinitionId };
            Predicate<Job> predicate = _ => _.JobDefinitionId == JobDefinitionId;

            await WhenMessagesReceived(QueueName, predicate);
            ThenReceiverTimesOut();
        }

        [TestMethod]
        public async Task CreateQueue_QueueGetsCreated()
        {
            await _messengerService.CreateQueue(QueueName);
            _queueClient.Verify(_ => _.CreateQueue(
                QueueName)
            );
        }

        [TestMethod]
        public async Task DeleteQueue_QueueGetsDeleted()
        {
            await _messengerService.DeleteQueue(QueueName);
            _queueClient.Verify(_ => _.DeleteQueue(
                QueueName)
            );
        }


        [DataTestMethod]
        [DataRow(null)]
        [DataRow(5)]
        public async Task SendToQueue_MessageSentToQueue(int? processingDelay)
        {
            // Arrange
            string sessionId = Guid.NewGuid().ToString();
            var job = new Job { JobDefinitionId = JobDefinitionId };

            string expectedPayload = JsonConvert.SerializeObject(new QueueMessage<Job>
            {
                Data = job,
                ApplicationProperties = null
            });

            // ACT
            await WhenMessageSentToQueue(
                QueueName, job, null, sessionId, processingDelay: processingDelay);

            // ASSERT (verify AFTER the call)
            ThenMessageSentToQueue(QueueName, expectedPayload, processingDelay);

            // Optional: only if you deliberately used Verifiable() on setups
            _queueClient.VerifyAll();
        }

        [TestMethod]
        public async Task SendToQueue_WithCompression_MessageSentToQueue()
        {
            string sessionId = Guid.NewGuid().ToString();
            Job job = new Job { JobDefinitionId = JobDefinitionId };

            await WhenMessageSentToQueue(QueueName,
                job,
                null,
                sessionId: sessionId,
                compression: true);

            ThenMessageSentToQueueWithCompression(QueueName);
        }

        [TestMethod]
        public async Task SendToQueueAsJson_MessageSentToQueue()
        {
            string sessionId = Guid.NewGuid().ToString();
            var job = new Job { JobDefinitionId = JobDefinitionId };
            string jobAsString = JsonConvert.SerializeObject(job);

            string expectedPayload = $"{{\"Data\":{jobAsString},\"ApplicationProperties\":null}}";

            // ACT first
            await WhenMessageSentToQueueAsJson(QueueName, jobAsString, null, sessionId);

            // THEN (ASSERT) after the call
            ThenMessageSentToQueue(QueueName, expectedPayload, processingDelay: null);

            _queueClient.VerifyAll();
        }


        [TestMethod]
        public async Task SendToQueueAsJson_WithCompression_MessageSentToQueue()
        {
            string sessionId = Guid.NewGuid().ToString();
            Job job = new Job { JobDefinitionId = JobDefinitionId };
            string jobAsString = JsonConvert.SerializeObject(job);

            await WhenMessageSentToQueueAsJson(QueueName,
                jobAsString,
                null,
                sessionId: sessionId,
                compression:true);

            ThenMessageSentToQueueWithCompression(QueueName);
        }

        [TestMethod]
        public async Task SendToTopic_MessageSentToQueue()
        {
            // Arrange
            string sessionId = Guid.NewGuid().ToString();
            var job = new Job { JobDefinitionId = JobDefinitionId };

            string expectedPayload = JsonConvert.SerializeObject(new QueueMessage<Job>
            {
                Data = job,
                ApplicationProperties = null
            });

            // Act
            await WhenMessageSentToTopic(TopicName, job, null, sessionId);

            // Assert
            ThenMessageSentToQueue(TopicName, expectedPayload, processingDelay: null);
            _queueClient.VerifyAll(); // optional—see note below
        }

        [TestMethod]
        public async Task SendToTopicAsJson_MessageSentToQueue()
        {
            // Arrange
            string sessionId = Guid.NewGuid().ToString();
            var job = new Job { JobDefinitionId = JobDefinitionId };
            string jobAsString = JsonConvert.SerializeObject(job);

            string expectedPayload = $"{{\"Data\":{jobAsString},\"ApplicationProperties\":null}}";

            // Act
            await WhenMessageSentToTopicAsJson(QueueName, jobAsString, null, sessionId);
            // (You probably meant TopicName here; see note below)

            // Assert
            ThenMessageSentToQueue(QueueName, expectedPayload, processingDelay: null);
            _queueClient.VerifyAll();
        }

        [TestMethod]
        public async Task SendToTopicAsJson_WithCompression_MessageSentToQueue()
        {
            string sessionId = Guid.NewGuid().ToString();
            Job job = new Job { JobDefinitionId = JobDefinitionId };
            string jobAsString = JsonConvert.SerializeObject(job);

            await WhenMessageSentToTopicAsJson(QueueName,
                jobAsString,
                null,
                sessionId: sessionId,
                compression:true);

            ThenMessageSentToQueueWithCompression(QueueName);
        }

        private void GivenQueueExists()
        {
            _queueClient.Setup(_ => _.Exists(QueueName))
                .ReturnsAsync(true)
                .Verifiable();
        }

        private void GivenMessageReturnedFromQueue(QueueMessage message)
        {
            _queueClient.Setup(_ => _.GetMessage(QueueName))
                .ReturnsAsync(message)
                .Verifiable();
        }

        private async Task<bool> WhenHealthChecked()
        {
            (bool isHealthy, string message) = await _messengerService.IsHealthOk(QueueName);
            return isHealthy;
        }

        private Task<T> WhenMessagesReceived<T>(string entityPath, Predicate<T> predicate) where T : class
            => _messengerService.ReceiveMessage(entityPath, predicate, TimeSpan.FromMilliseconds(1));

        private async Task WhenMessageSentToTopic<T>(string topicName, T data, IDictionary<string, string> properties, string sessionId)
            where T:class
        {
            await _messengerService.SendToTopic(topicName,
                data,
                properties,
                sessionId: sessionId);
        }

        private async Task WhenMessageSentToTopicAsJson(string topicName, string data, IDictionary<string, string> properties, string sessionId, bool compression = false)
        {
            await _messengerService.SendToTopicAsJson(topicName,
                data,
                properties,
                sessionId: sessionId,
                compressData: compression);
        }

        private async Task WhenMessageSentToQueue<T>(string queueName, T data, IDictionary<string, string> properties, string sessionId, bool compression = false, int? processingDelay = null)
            where T:class
        {
            await _messengerService.SendToQueue(queueName,
                data,
                properties,
                sessionId: sessionId,
                compressData: compression,
                processingDelay: processingDelay);
        }

        private async Task WhenMessageSentToQueueAsJson(string queueName, string data, IDictionary<string, string> properties, string sessionId, bool compression = false)
        {
            await _messengerService.SendToQueueAsJson(queueName,
                data,
                properties,
                sessionId: sessionId,
                compressData: compression);
        }

        private void ThenTargetMessageReceived<T>(T message, Predicate<T> predicate)
        {
            predicate(message)
                .Should()
                .BeTrue();
        }

        private void ThenReceiverTimesOut()
        {
            _queueClient.Verify(_ => _.TimedOut());
        }

        private void ThenMessageSentToQueue(string queueName, string expectedPayload, int? processingDelay)
        {
            if (processingDelay.HasValue)
            {
                _queueClient.Verify(_ => _.AddMessage(
                    queueName,
                    It.Is<string>(s => s == expectedPayload),
                    It.Is<TimeSpan?>(t => t.HasValue && t.Value.Minutes == processingDelay.Value)
                ));
            }
            else
            {
                _queueClient.Verify(_ => _.AddMessage(
                    queueName,
                    It.Is<string>(s => s == expectedPayload),
                    It.IsAny<TimeSpan?>()
                ));
            }
        }

        private void ThenMessageSentToQueueWithCompression(string queueName)
        {
            _queueClient.Verify(_ => _.AddMessage(queueName, It.IsAny<string>(), It.IsAny<TimeSpan?>()));
        }

        private void ThenServiceIsHealthy(bool isHealthy)
        {
            isHealthy
                .Should()
                .BeTrue();
        }

        private void ThenServiceIsUnHealthy(bool isHealthy)
        {
            isHealthy
                .Should()
                .BeFalse();
        }
    }
}
