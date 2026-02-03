using Azure.Messaging.ServiceBus;
using CalculateFunding.Common.ServiceBus.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ServiceBus
{
    public class AzureMessageReceiver : IMessageReceiver
    {
        private readonly ServiceBusReceiver _receiver;

        public AzureMessageReceiver(ServiceBusReceiver receiver)
        {
            _receiver = receiver;
        }

        public Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessagesAsync(int maxMessages)
            => _receiver.PeekMessagesAsync(maxMessages);

        public Task<ServiceBusReceivedMessage> ReceiveMessageAsync(TimeSpan timeout)
            => _receiver.ReceiveMessageAsync(timeout);

        public Task CompleteMessageAsync(ServiceBusReceivedMessage message)
            => _receiver.CompleteMessageAsync(message);

        public Task CloseAsync()
                => _receiver.CloseAsync();

    }
}
