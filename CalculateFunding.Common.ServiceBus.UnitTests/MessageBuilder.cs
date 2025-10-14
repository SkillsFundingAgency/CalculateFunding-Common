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
            _message = new ServiceBusMessage(BinaryData.FromBytes(body));

            return this;
        }

        public MessageBuilder WithLockToken(Guid lockToken)
        {
            //Todo - dotnet - correct this for the tests
            //Message.SystemPropertiesCollection systemProperties = _message.SystemProperties;
            //Type type = systemProperties.GetType();
            //type.GetMethod("set_LockTokenGuid", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(systemProperties, new object[] { lockToken });
            //type.GetMethod("set_SequenceNumber", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(systemProperties, new object[] { 0 });

            return this;
        }

        public ServiceBusMessage Build()
        {
            return _message;
        }
    }
}