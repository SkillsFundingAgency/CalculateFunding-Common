using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.ServiceBus.Interfaces;
using CalculateFunding.Common.ServiceBus.Options;
using CalculateFunding.Common.Utility;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using AzureCore = Microsoft.Azure.ServiceBus.Core;
using AzureServiceBus = Microsoft.Azure.ServiceBus;

namespace CalculateFunding.Common.ServiceBus
{
    public class MessengerService : BaseMessengerService, IMessengerService, IServiceBusService
    {
        private readonly IManagementClient _managementClient;

        public string ServiceName { get; }

        private IMessageReceiverFactory _messageReceiverFactory;

        public MessengerService(ServiceBusSettings settings, IManagementClient managementClient, IMessageReceiverFactory messageReceiverFactory, string serviceName = null)
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
                AzureCore.IMessageReceiver receiver = _messageReceiverFactory.Receiver(queueName);
                IList<Message> message = await receiver.PeekAsync(1);
                await receiver.CloseAsync();
                return await Task.FromResult((true, string.Empty));
            }
            catch (ServiceBusCommunicationException ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task CreateSubscription(string topicName, string subscriptionName, TimeSpan timeSpan)
        {
            await _managementClient.CreateSubscription(topicName, subscriptionName, timeSpan);
        }

        public async Task CreateSubscription(string topicName, string subscriptionName)
        {
            await _managementClient.CreateSubscription(topicName, subscriptionName);
        }

        public async Task DeleteSubscription(string topicName, string subscriptionName)
        {
            await _managementClient.DeleteSubscription(topicName, subscriptionName);
        }

        public async Task CreateQueue(string queuePath)
        {
            await _managementClient.CreateQueue(queuePath);
        }

        public async Task DeleteQueue(string queuePath)
        {
            await _managementClient.DeleteQueue(queuePath);
        }

        public async Task CreateTopic(string topicName)
        {
            await _managementClient.CreateTopic(topicName);
        }

        public async Task DeleteTopic(string topicName)
        {
            await _managementClient.DeleteTopic(topicName);
        }

        public async Task SendToQueue<T>(string queueName,
            T data,
            IDictionary<string, string> properties,
            bool compressData = false,
            string sessionId = null) where T : class
        {
            string json = JsonConvert.SerializeObject(data);

            await SendToQueueAsJson(queueName, json, properties, compressData, sessionId);
        }

        public async Task SendToQueueAsJson(string queueName,
            string data,
            IDictionary<string, string> properties,
            bool compressData = false,
            string sessionId = null)
        {
            Guard.IsNullOrWhiteSpace(queueName, nameof(queueName));

            Message message = ConstructMessage(data, compressData, sessionId);

            foreach (KeyValuePair<string, string> property in properties)
            {
                message.UserProperties.Add(property.Key, property.Value);
            }

            AzureServiceBus.IQueueClient queueClient = _managementClient.GetQueueClient(queueName);

            await queueClient.SendAsync(message);
        }

        public async Task SendToTopic<T>(string topicName,
            T data,
            IDictionary<string, string> properties,
            bool compressData = false,
            string sessionId = null) where T : class
        {
            string json = JsonConvert.SerializeObject(data);

            await SendToTopicAsJson(topicName, json, properties, compressData, sessionId);
        }

        public async Task SendToTopicAsJson(string topicName,
            string data,
            IDictionary<string, string> properties,
            bool compressData = false,
            string sessionId = null)
        {
            Guard.IsNullOrWhiteSpace(topicName, nameof(topicName));

            Message message = ConstructMessage(data, compressData, sessionId);

            foreach (KeyValuePair<string, string> property in properties)
            {
                message.UserProperties.Add(property.Key, property.Value);
            }

            ITopicClient topicClient = _managementClient.GetTopicClient(topicName);

            await topicClient.SendAsync(message);
        }

        protected async override Task ReceiveMessages<T>(string entityPath, Predicate<T> predicate, TimeSpan timeout)
        {
            List<T> messages = new List<T>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            AzureCore.IMessageReceiver receiver = _messageReceiverFactory.Receiver(entityPath);

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
                    Message message = await receiver.ReceiveAsync(TimeSpan.FromSeconds(5));

                    try
                    {
                        if (message != null)
                        {
                            await receiver.CompleteAsync(message.SystemProperties.LockToken);
                            string json = null;

                            using (MemoryStream inputStream = new MemoryStream(message.Body))
                            {
                                using (StreamReader streamReader = new StreamReader(inputStream))
                                {
                                    json = streamReader.ReadToEnd();
                                }
                            }

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

        private static Message ConstructMessage(string data, bool compressData, string sessionId = null)
        {
            Message message = null;

            if (!string.IsNullOrWhiteSpace(data))
            {
                byte[] bytes = compressData ? data.Compress() : Encoding.UTF8.GetBytes(data);

                message = new Message(bytes);

                if (compressData)
                {
                    message.ContentType = "application/gzip";
                }
            }
            else
            {
                message = new Message();
            }

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                message.SessionId = sessionId;
            }

            return message;
        }
    }
}
