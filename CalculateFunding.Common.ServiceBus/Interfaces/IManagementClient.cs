using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Threading.Tasks;
using AzureManagement = Azure.Messaging.ServiceBus.Administration;
using AzureServiceBus = Azure.Messaging.ServiceBus;

namespace CalculateFunding.Common.ServiceBus.Interfaces
{
    public interface IManagementClient
    {
        ServiceBusSender GetTopicSender(string topicName);

        AzureServiceBus.ServiceBusSender GetQueueSender(string queueName);

        Task CreateSubscriptionAsync(string topicName, string subscriptionName, TimeSpan timeSpan);

        Task CreateSubscriptionAsync(string topicName, string subscriptionName);

        Task DeleteSubscriptionAsync(string topicName, string subscriptionName);

        Task<AzureManagement.QueueProperties> CreateQueueAsync(string queuePath);

        Task DeleteQueueAsync(string queuePath);

        Task<TopicProperties> CreateTopicAsync(string topicName);

        Task DeleteTopicAsync(string topicName);
    }
}
