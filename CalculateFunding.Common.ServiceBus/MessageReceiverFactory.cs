using CalculateFunding.Common.ServiceBus.Interfaces;
using Azure.Messaging.ServiceBus;

namespace CalculateFunding.Common.ServiceBus
{
    public class MessageReceiverFactory : IMessageReceiverFactory
    {
        private readonly ServiceBusClient _client;

        public MessageReceiverFactory(string connectionString)
        {
            _client = new ServiceBusClient(connectionString);
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
