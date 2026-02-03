using CalculateFunding.Common.Testing;
using Azure.Messaging.ServiceBus;
using System;

namespace CalculateFunding.Common.ServiceBus.UnitTests
{
    public class MessageBuilder : TestEntityBuilder
    {
        private ServiceBusMessage _message = new ServiceBusMessage();

        public MessageBuilder WithUserProperty(string key, string value)
        {
            _message.ApplicationProperties.Add(key, value);

            return this;
        }

        public MessageBuilder WithoutUserProperty(string key)
        {
            _message.ApplicationProperties.Remove(key);

            return this;
        }

        public MessageBuilder WithBody(byte[] body)
        {
            _message.Body = BinaryData.FromBytes(body);

            return this;
        }

        public MessageBuilder WithLockToken(Guid lockToken)
        {
            return this;
        }

        public ServiceBusMessage Build()
        {
            return _message;
        }
    }
}