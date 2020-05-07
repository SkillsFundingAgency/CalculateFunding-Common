using System.Collections.Generic;

namespace CalculateFunding.Common.ServiceBus
{
    public class QueueMessage<T> where T : class
    {
        public QueueMessage()
        {
            UserProperties = new Dictionary<string, string>();
        }

        public IDictionary<string, string> UserProperties { get; set; }

        public T Data { get; set; }
    }
}
