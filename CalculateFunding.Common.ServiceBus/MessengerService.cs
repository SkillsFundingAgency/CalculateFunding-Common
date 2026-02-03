using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.ServiceBus.Interfaces;
using CalculateFunding.Common.Utility;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ServiceBus
{
    public class MessengerService : BaseMessengerService, IMessengerService, IServiceBusService
    {
        private readonly IManagementClient _managementClient;

        public string ServiceName { get; }

        private IMessageReceiverFactory _messageReceiverFactory;

        public MessengerService(IManagementClient managementClient, IMessageReceiverFactory messageReceiverFactory, string serviceName = null)
        {
            _managementClient = managementClient;
            _messageReceiverFactory = messageReceiverFactory;
            ServiceName = serviceName;
        }

        public async Task<(bool Ok, string Message)> IsHealthOk(string queueName)
        {
            try
            {
                // Only way to check if connection string is correct is try receiving a message, 
                // which isn't possible for topics as don't have a subscription
                IMessageReceiver receiver = _messageReceiverFactory.Receiver(queueName);
                IReadOnlyList<ServiceBusReceivedMessage> messages = await receiver.PeekMessagesAsync(1);
                return (true, string.Empty);
            }
            catch (ServiceBusException ex) when (ex.Reason == ServiceBusFailureReason.ServiceCommunicationProblem)
            {
                return (false, ex.Message);
            }
        }

        public async Task CreateSubscription(string topicName, string subscriptionName, TimeSpan timeSpan)
        {
            await _managementClient.CreateSubscriptionAsync(topicName, subscriptionName, timeSpan);
        }

        public async Task CreateSubscription(string topicName, string subscriptionName)
        {
            await _managementClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }

        public async Task DeleteSubscription(string topicName, string subscriptionName)
        {
            await _managementClient.DeleteSubscriptionAsync(topicName, subscriptionName);
        }

        public async Task CreateQueue(string queuePath)
        {
            await _managementClient.CreateQueueAsync(queuePath);
        }

        public async Task DeleteQueue(string queuePath)
        {
            await _managementClient.DeleteQueueAsync(queuePath);
        }

        public async Task CreateTopic(string topicName)
        {
            await _managementClient.CreateTopicAsync(topicName);
        }

        public async Task DeleteTopic(string topicName)
        {
            await _managementClient.DeleteTopicAsync(topicName);
        }

        public async Task SendToQueue<T>(string queueName,
            T data,
            IDictionary<string, string> properties,
            bool compressData = false,
            string sessionId = null,
            int? processingDelay = null) where T : class
        {
            string json = JsonConvert.SerializeObject(data);

            await SendToQueueAsJson(queueName, json, properties, compressData, sessionId, processingDelay);
        }

        public async Task SendToQueueAsJson(string queueName,
            string data,
            IDictionary<string, string> properties,
            bool compressData = false,
            string sessionId = null,
            int? processingDelay = null)
        {
            Guard.IsNullOrWhiteSpace(queueName, nameof(queueName));

            ServiceBusMessage message = ConstructMessage(data, compressData, sessionId);


            foreach (KeyValuePair<string, string> property in properties)
            {
                message.ApplicationProperties.Add(property.Key, property.Value);
            }

            if (processingDelay.HasValue)
            {
                message.ScheduledEnqueueTime = DateTimeOffset.UtcNow.AddMinutes(processingDelay.Value);
            }

            ServiceBusSender queueClient = _managementClient.GetQueueSender(queueName);

            await queueClient.SendMessageAsync(message);
        }

        public async Task SendToTopic<T>(string topicName,
            T data,
            IDictionary<string, string> properties,
            bool compressData = false,
            string sessionId = null,
            int? processingDelay = null) where T : class
        {
            string json = JsonConvert.SerializeObject(data);

            await SendToTopicAsJson(topicName, json, properties, compressData, sessionId, processingDelay);
        }

        public async Task SendToTopicAsJson(string topicName,
            string data,
            IDictionary<string, string> properties,
            bool compressData = false,
            string sessionId = null,
            int? processingDelay = null)
        {
            Guard.IsNullOrWhiteSpace(topicName, nameof(topicName));

            ServiceBusMessage message = ConstructMessage(data, compressData, sessionId);

            foreach (KeyValuePair<string, string> property in properties)
            {
                message.ApplicationProperties.Add(property.Key, property.Value);
            }

            if (processingDelay.HasValue)
            {
                message.ScheduledEnqueueTime = DateTimeOffset.UtcNow.AddMinutes(processingDelay.Value);
            }

            ServiceBusSender topicClient = _managementClient.GetTopicSender(topicName);

            await topicClient.SendMessageAsync(message);
        }

        protected async override Task ReceiveMessages<T>(string entityPath, Predicate<T> predicate, TimeSpan timeout)
        {
            List<T> messages = new List<T>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            IMessageReceiver receiver = _messageReceiverFactory.Receiver(entityPath);

            _ = Task.Run(() =>
            {
                if (!cancellationTokenSource.Token.WaitHandle.WaitOne(timeout))
                {
                    _messageReceiverFactory.TimedOut();
                    cancellationTokenSource.Cancel();
                }
            });

            try
            {
                while (true)
                {
                    ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(5));

                    try
                    {
                        if (message != null)
                        {
                            await receiver.CompleteMessageAsync(message);
                            string json = message.Body.ToString();

                            T messageOfType = JsonConvert.DeserializeObject<T>(json);

                            if (predicate(messageOfType))
                            {
                                break;
                            }
                        }
                    }
                    catch (JsonSerializationException)
                    {
                        // don't do anything the serialization failed as this is not a message we are interested in
                    }

                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        break;
                }
            }
            finally
            {
                // cancel timeout
                cancellationTokenSource.Cancel();
                await receiver.CloseAsync();
            }
        }

        private static ServiceBusMessage ConstructMessage(string data, bool compressData, string sessionId = null)
        {
            ServiceBusMessage message = null;

            if (!string.IsNullOrWhiteSpace(data))
            {
                byte[] bytes = compressData ? data.Compress() : Encoding.UTF8.GetBytes(data);

                message = new ServiceBusMessage(bytes);

                if (compressData)
                {
                    message.ContentType = "application/gzip";
                }
            }
            else
            {
                message = new ServiceBusMessage();
            }

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                message.SessionId = sessionId;
            }

            return message;
        }
    }
}
