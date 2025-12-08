using System.Collections.Generic;

namespace CalculateFunding.Common.ServiceBus
{
    public class QueueMessage<T> where T : class
    {
        public QueueMessage()
        {
            ApplicationProperties = new Dictionary<string, string>();
        }

        public IDictionary<string, string> ApplicationProperties { get; set; }

        public T Data { get; set; }
    }
}
