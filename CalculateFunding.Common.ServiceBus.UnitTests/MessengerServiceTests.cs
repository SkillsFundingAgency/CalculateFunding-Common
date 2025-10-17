using Azure.Messaging.ServiceBus;
using CalculateFunding.Common.ApiClient.Jobs.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.ServiceBus.Interfaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;


namespace CalculateFunding.Common.ServiceBus.UnitTests
{
    [TestClass]
    public class MessengerServiceTests
    {
        private MessengerService _messengerService;
        private Mock<IMessageReceiverFactory> _messageReceiverFactory;
        private Mock<IMessageReceiver> _azureMessageReceiver;
        private Mock<IManagementClient> _managementClient;
        private Mock<ServiceBusSender> _topicClient;
        private Mock<ServiceBusSender> _queueClient;
        private const string JobDefinitionId = "PopulateScopedProvidersJob";
        private const string TopicName = "TopicName";
        private const string SubscriptionName = "SubscriptionName";
        private const string QueueName = "QueueName";

        [TestInitialize]
        public void SetUp()
        {
            _azureMessageReceiver = new Mock<IMessageReceiver>();
            _managementClient = new Mock<IManagementClient>();
            _messageReceiverFactory = new Mock<IMessageReceiverFactory>();
            _topicClient = new Mock<ServiceBusSender>();
            _queueClient = new Mock<ServiceBusSender>();
            _messengerService = new MessengerService(_managementClient.Object,
                _messageReceiverFactory.Object);
        }

        [TestMethod]
        public async Task IsHealthy_PeekSuccessful_ReturnsTrue()
        {
            GivenMessageReceiverReturned(QueueName);
            GivenPeekSuccessful();
            bool isHealthy = await WhenHealthChecked();
            ThenServiceIsHealthy(isHealthy);
            _azureMessageReceiver.VerifyAll();
            _messageReceiverFactory.VerifyAll();
        }

        [TestMethod]
        public async Task IsHealthy_PeekUnsuccessful_ReturnsTrue()
        {
            GivenMessageReceiverReturned(QueueName);
            bool isHealthy = await WhenHealthChecked();
            ThenServiceIsUnhealthy(isHealthy);
            _messageReceiverFactory.VerifyAll();
        }

        [TestMethod]
        public async Task ReceiveMessages_TargetMessageReturnedFromServiceBus_TargetMessageReceived()
        {
            Guid lockToken = Guid.NewGuid();
            Job job = new Job { JobDefinitionId = JobDefinitionId };
            Predicate<Job> predicate = _ => _.JobDefinitionId == JobDefinitionId;

            _messageReceiverFactory.Setup(_ => _.Receiver(QueueName))
                .Returns((IMessageReceiver)_azureMessageReceiver.Object);

            ServiceBusMessage message = NewMessage(_ => _.WithLockToken(lockToken)
                .WithBody(JsonConvert.SerializeObject(job).AsUTF8Bytes()));

            GivenMessageReceiverReturned(QueueName);
            AndMessageReturnedFromServiceBus(message);
            Job jobReceived = await WhenMessagesReceived(QueueName, predicate);
            ThenTargetMessageReceived(job, predicate);
            _azureMessageReceiver.VerifyAll();
            _messageReceiverFactory.VerifyAll();
        }

        [TestMethod]
        public async Task ReceiveMessages_TargetMessageNotReturnedFromServiceBus_TimesOut()
        {
            Guid lockToken = Guid.NewGuid();
            Job job = new Job { JobDefinitionId = JobDefinitionId };
            Predicate<Job> predicate = _ => _.JobDefinitionId == JobDefinitionId;

            ServiceBusMessage ignoreMessage = NewMessage(_ => _.WithLockToken(lockToken)
                .WithBody(JsonConvert.SerializeObject(Guid.NewGuid()).AsUTF8Bytes()));

            GivenMessageReceiverReturned(QueueName);
            AndMessageReturnedFromServiceBus(ignoreMessage);
            await WhenMessagesReceived(QueueName, predicate);
            ThenReceiverTimesOut();
            _messageReceiverFactory.VerifyAll();
        }

        [TestMethod]
        public async Task CreateSubscription_SubscriptionGetsCreated()
        {
            await _messengerService.CreateSubscription(TopicName, SubscriptionName);
            _managementClient.Verify(_ => _.CreateSubscriptionAsync(
                TopicName,
                SubscriptionName)
            );
        }

        [TestMethod]
        public async Task DeleteSubscription_SubscriptionGetsDeleted()
        {
            await _messengerService.DeleteSubscription(TopicName, SubscriptionName);
            _managementClient.Verify(_ => _.DeleteSubscriptionAsync(
                TopicName,
                SubscriptionName)
            );
        }

        [TestMethod]
        public async Task CreateQueue_QueueGetsCreated()
        {
            await _messengerService.CreateQueue(QueueName);
            _managementClient.Verify(_ => _.CreateQueueAsync(
                QueueName)
            );
        }

        [TestMethod]
        public async Task DeleteQueue_QueueGetsDeleted()
        {
            await _messengerService.DeleteQueue(QueueName);
            _managementClient.Verify(_ => _.DeleteQueueAsync(
                QueueName)
            );
        }

        [TestMethod]
        public async Task CreateTopic_TopicGetsCreated()
        {
            await _messengerService.CreateTopic(TopicName);
            _managementClient.Verify(_ => _.CreateTopicAsync(
                TopicName)
            );
        }

        [TestMethod]
        public async Task DeleteTopic_TopicGetsDeleted()
        {
            await _messengerService.DeleteTopic(TopicName);
            _managementClient.Verify(_ => _.DeleteTopicAsync(
                TopicName)
            );
        }

        [TestMethod]
        [DataRow(true, null)]
        [DataRow(false, null)]
        [DataRow(false, 5)]
        public async Task SendToTopic_MessageSentToTopic(bool compressData, int? processingDelay)
        {
            string sessionId = Guid.NewGuid().ToString();
            Job job = new Job { JobDefinitionId = JobDefinitionId };
            ServiceBusMessage message = NewMessage(_ => _.WithUserProperty("job_id", "job_id"));

            GivenTopic(TopicName);

            await WhenMessageSentToTopic(TopicName,
                job,
                message.ApplicationProperties.ToDictionary(_ => _.Key, _ => _.Value.ToString()),
                sessionId: sessionId,
                compressData: compressData,
                processingDelay: processingDelay);

            ThenMessageSentToTopic(sessionId, compressData, processingDelay);
            _managementClient.VerifyAll();
        }

        [TestMethod]
        public async Task SendToQueue_MessageSentToQueue()
        {
            string sessionId = Guid.NewGuid().ToString();
            Job job = new Job { JobDefinitionId = JobDefinitionId };
            ServiceBusMessage message = NewMessage(_ => _.WithUserProperty("job_id", "job_id"));

            GivenQueue(QueueName);

            await WhenMessageSentToQueue(QueueName,
                job,
                message.ApplicationProperties.ToDictionary(_ => _.Key, _ => _.Value.ToString()),
                sessionId: sessionId);

            ThenMessageSentToQueue(sessionId);
            _managementClient.VerifyAll();
        }

        private void GivenPeekSuccessful()
        {
            _azureMessageReceiver.Setup(_ => _.PeekMessagesAsync(1))
            .ReturnsAsync(new List<ServiceBusReceivedMessage>())
            .Verifiable();
        }

        private void GivenTopic(string topicName)
        {
            _managementClient.Setup(_ => _.GetTopicSender(It.Is<string>(_ => _ == topicName)))
                .Returns(_topicClient.Object)
                .Verifiable(); ;
        }

        private void GivenQueue(string queueName)
        {
            _managementClient.Setup(_ => _.GetQueueSender(It.Is<string>(_ => _ == queueName)))
                .Returns(_queueClient.Object)
                .Verifiable();
        }

        private void GivenMessageReceiverReturned(string queueName)
        {
            _messageReceiverFactory.Setup(_ => _.Receiver(queueName))
                .Returns(_azureMessageReceiver.Object)
                .Verifiable();
        }

        private void AndMessageReturnedFromServiceBus(ServiceBusMessage message)
        {
            _azureMessageReceiver.Verify(_ => _.ReceiveMessageAsync(
                    It.Is<TimeSpan>(t => t == TimeSpan.FromSeconds(5))),
                Times.Once);
        }

        private async Task<bool> WhenHealthChecked()
        {
            (bool isHealthy, string message) = await _messengerService.IsHealthOk(QueueName);

            return isHealthy;
        }

        private async Task<T> WhenMessagesReceived<T>(string entityPath, Predicate<T> predicate)
            where T:class
        {
            return await _messengerService.ReceiveMessage(entityPath, predicate, TimeSpan.FromMilliseconds(1));
        }

        private async Task WhenMessageSentToTopic<T>(string topicName, T data, IDictionary<string, string> properties, string sessionId, bool compressData = false, int? processingDelay = null)
            where T:class
        {
            await _messengerService.SendToTopic<T>(topicName,
                data,
                properties,
                sessionId: sessionId,
                compressData: compressData,
                processingDelay: processingDelay);
        }

        private async Task WhenMessageSentToQueue<T>(string queueName, T data, IDictionary<string, string> properties, string sessionId)
            where T : class
        {
            await _messengerService.SendToQueue(queueName,
                data,
                properties,
                sessionId: sessionId);
        }

        private void ThenMessageSentToTopic(string sessionId, bool compressData = false, int? processingDelay = null)
        {
            if (compressData)
            {
                _topicClient.Verify(s => s.SendMessageAsync(
                        It.Is<ServiceBusMessage>(m =>
                            m.SessionId == sessionId &&
                            m.ContentType == "application/gzip"),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
            }
            else
            {
                if (processingDelay.HasValue)
                {
                    var expected = DateTimeOffset.UtcNow.AddMinutes(processingDelay.Value);
                    var lower = expected.AddSeconds(-5);
                    var upper = expected.AddSeconds(5);

                    _topicClient.Verify(s => s.ScheduleMessageAsync(
                            It.Is<ServiceBusMessage>(m => m.SessionId == sessionId),
                            It.Is<DateTimeOffset>(t => t >= lower && t <= upper),
                            It.IsAny<CancellationToken>()),
                        Times.Once);
                }
                else
                {
                    _topicClient.Verify(s => s.SendMessageAsync(
                        It.Is<ServiceBusMessage>(m => m.SessionId == sessionId),
                        It.IsAny<CancellationToken>()),
                        Times.Once);
                }
            }
        }

        private void ThenMessageSentToQueue(string sessionId)
        {
            ServiceBusMessage? captured = null;

            _queueClient
                .Setup(q => q.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()))
                .Callback<ServiceBusMessage, CancellationToken>((m, _) => captured = m)
                .Returns(Task.CompletedTask);

            captured.Should().NotBeNull();
            captured!.SessionId.Should().Be(sessionId);
        }

        private void ThenTargetMessageReceived<T>(T message, Predicate<T> predicate)
        {
            predicate(message)
                .Should()
                .BeTrue();
        }

        private void ThenReceiverTimesOut()
        {
            _messageReceiverFactory.Verify(_ => _.TimedOut());
        }

        private void ThenServiceIsHealthy(bool isHealthy)
        {
            isHealthy
                .Should()
                .BeTrue();
        }

        private void ThenServiceIsUnhealthy(bool isHealthy)
        {
            isHealthy
                .Should()
                .BeTrue();
        }

        private ServiceBusMessage NewMessage(Action<MessageBuilder> setup = null)
        {
            MessageBuilder messageBuilder = new MessageBuilder();

            setup?.Invoke(messageBuilder);

            return messageBuilder.Build();
        }
    }
}
