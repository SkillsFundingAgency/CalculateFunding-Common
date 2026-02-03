using CalculateFunding.Common.ServiceBus.Interfaces;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ServiceBus
{
    public class ManagementClient : IManagementClient
    {
        private static readonly ConcurrentDictionary<string, ServiceBusSender> _queueSenders = new();
        private static readonly ConcurrentDictionary<string, ServiceBusSender> _topicSenders = new();
        private readonly string _connectionString;

        private ServiceBusAdministrationClient _adminClient => new ServiceBusAdministrationClient(_connectionString);
        public ManagementClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ServiceBusSender GetTopicSender(string topicName)
        {
            return _topicSenders.GetOrAdd(topicName, key =>
            {
                var client = new ServiceBusClient(_connectionString);
                return client.CreateSender(key);
            });
        }

        public ServiceBusSender GetQueueSender(string queueName)
        {
            return _queueSenders.GetOrAdd(queueName, key =>
            {
                var client = new ServiceBusClient(_connectionString);
                return client.CreateSender(key);
            });
        }

        public async Task CreateSubscriptionAsync(string topicName, string subscriptionName, TimeSpan timeSpan)
        {
            var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName)
            {
                DefaultMessageTimeToLive = timeSpan
            };
            if (!await _adminClient.SubscriptionExistsAsync(topicName, subscriptionName))
            {
                await _adminClient.CreateSubscriptionAsync(subscriptionOptions);
            }
        }


        public async Task CreateSubscriptionAsync(string topicName, string subscriptionName)
        {
            if (!await _adminClient.SubscriptionExistsAsync(topicName, subscriptionName))
            {
                await _adminClient.CreateSubscriptionAsync(topicName, subscriptionName);
            }
        }

        public async Task DeleteSubscriptionAsync(string topicName, string subscriptionName)
        {
            if (await _adminClient.SubscriptionExistsAsync(topicName, subscriptionName))
            {
                await _adminClient.DeleteSubscriptionAsync(topicName, subscriptionName);
            }

        }

        public async Task<QueueProperties> CreateQueueAsync(string queuePath)
        {
            if (!await _adminClient.QueueExistsAsync(queuePath))
            {
                return await _adminClient.CreateQueueAsync(queuePath);
            }
            return await _adminClient.GetQueueAsync(queuePath);
        }

        public async Task DeleteQueueAsync(string queuePath)
        {
            if (await _adminClient.QueueExistsAsync(queuePath))
            {
                await _adminClient.DeleteQueueAsync(queuePath);
            }

        }

        public async Task<TopicProperties> CreateTopicAsync(string topicName)
        {
            if (!await _adminClient.TopicExistsAsync(topicName))
            {
                return await _adminClient.CreateTopicAsync(topicName);
            }
            return await _adminClient.GetTopicAsync(topicName);
        }


        public async Task DeleteTopicAsync(string topicName)
        {
            if (await _adminClient.TopicExistsAsync(topicName))
            {
                await _adminClient.DeleteTopicAsync(topicName);
            }

        }
    }
}
