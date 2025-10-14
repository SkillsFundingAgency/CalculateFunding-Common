using Azure.Messaging.ServiceBus;

namespace CalculateFunding.Common.ServiceBus.Interfaces
{
    public interface IMessageReceiverFactory
    {
        ServiceBusReceiver Receiver(string queueName);
        void TimedOut();
    }
}
