using Azure.Storage.Queues.Models;
using Azure.Storage.Queues;
using Azure;
using CalculateFunding.Common.ServiceBus.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ServiceBus
{
    public class StorageQueueManager : IQueueClient
    {

        private static readonly ConcurrentDictionary<string, QueueClient> _queues =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly QueueServiceClient _serviceClient;

        public StorageQueueManager(string connectionString)
        {
            _serviceClient = new QueueServiceClient(connectionString);
        }

        private QueueClient GetQueueClient(string queueName)
        {
            return _queues.GetOrAdd(queueName, name => _serviceClient.GetQueueClient(name));
        }

        public async Task CreateQueue(string entityPath)
        {
            var queue = GetQueueClient(entityPath);
            await queue.CreateIfNotExistsAsync();
        }

        public async Task<bool> Exists(string entityPath)
        {
            var queue = GetQueueClient(entityPath);
            Response<bool> exists = await queue.ExistsAsync();
            return exists.Value;
        }

        public async Task DeleteQueue(string entityPath)
        {
            var queue = GetQueueClient(entityPath);
            await queue.DeleteAsync();
        }

        public async Task<QueueMessage?> GetMessage(string entityPath)
        {
            var queue = GetQueueClient(entityPath);

            var response = await queue.ReceiveMessageAsync();
            return response.Value; // null if no message
        }

        public async Task DeleteMessage(string entityPath, QueueMessage message)
        {
            var queue = GetQueueClient(entityPath);
            // v12 requires MessageId + PopReceipt (returned by Receive/Update)
            await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }

        public async Task AddMessage(string entityPath, string messageText, TimeSpan? initialVisibilityDelay = null)
        {
            var queue = GetQueueClient(entityPath);
            await queue.CreateIfNotExistsAsync();

            await queue.SendMessageAsync(
                messageText,
                timeToLive: null,                 // null => service default TTL (typically 7 days)
                visibilityTimeout: initialVisibilityDelay);
        }

        public void TimedOut() { /* parity no-op */ }
    }
}
