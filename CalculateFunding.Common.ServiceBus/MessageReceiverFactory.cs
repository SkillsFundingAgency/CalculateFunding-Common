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

        public ServiceBusReceiver Receiver(string queueName)
        {
            return _client.CreateReceiver(queueName);
        }


        public void TimedOut()
        {
        }
    }
}
