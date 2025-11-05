using Azure.Storage.Queues.Models;
using System;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ServiceBus.Interfaces
{
    public interface IQueueClient
    {
        Task CreateQueue(string entityPath);
        Task DeleteQueue(string entityPath);
        Task<QueueMessage?> GetMessage(string entityPath);
        Task DeleteMessage(string entityPath, QueueMessage message);
        Task AddMessage(string entityPath, string messageText, TimeSpan? initialVisibilityDelay = null);
        Task<bool> Exists(string entityPath);
        void TimedOut();
    }
}
