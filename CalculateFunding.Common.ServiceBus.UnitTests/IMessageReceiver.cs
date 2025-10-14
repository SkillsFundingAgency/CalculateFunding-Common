using Azure.Messaging.ServiceBus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ServiceBus.UnitTests
{
    public interface IMessageReceiver
    {
        Task<IReadOnlyList<ServiceBusReceivedMessage>> PeekMessagesAsync(int maxMessages);
    }
}
