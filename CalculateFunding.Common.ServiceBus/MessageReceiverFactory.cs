using CalculateFunding.Common.ServiceBus.Interfaces;
using Azure.Messaging.ServiceBus;
using System;

namespace CalculateFunding.Common.ServiceBus
{
    public class MessageReceiverFactory : IMessageReceiverFactory
    {
        private readonly ServiceBusClient _client;

        public MessageReceiverFactory(string connectionString)
        {
            _client = new ServiceBusClient(connectionString,
                new ServiceBusClientOptions
                {
                    TransportType = ServiceBusTransportType.AmqpWebSockets,
                    RetryOptions = new ServiceBusRetryOptions
                    {
                        TryTimeout = TimeSpan.FromSeconds(120)
                    }
                });
        }

        public IMessageReceiver Receiver(string queueName)
        {
            var receiver = _client.CreateReceiver(queueName);
            return new AzureMessageReceiver(receiver);

        }


        public void TimedOut()
        {
        }
    }
}
