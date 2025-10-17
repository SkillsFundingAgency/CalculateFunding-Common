using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ServiceBus.Interfaces
{
    public interface IMessageReceiver
    {
        Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessagesAsync(int maxMessages);
        Task<ServiceBusReceivedMessage> ReceiveMessageAsync(TimeSpan timeout);
        Task CompleteMessageAsync(ServiceBusReceivedMessage message);
        Task CloseAsync();

    }
}
