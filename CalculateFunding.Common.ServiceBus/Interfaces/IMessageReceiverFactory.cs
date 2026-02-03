using Azure.Messaging.ServiceBus;

namespace CalculateFunding.Common.ServiceBus.Interfaces
{
    public interface IMessageReceiverFactory
    {
        IMessageReceiver Receiver(string queueName);
        void TimedOut();
    }
}
