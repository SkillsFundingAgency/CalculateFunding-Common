﻿using CalculateFunding.Common.ServiceBus.Interfaces;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Collections.Generic;
using System.Text;
using AzureCore = Microsoft.Azure.ServiceBus.Core;

namespace CalculateFunding.Common.ServiceBus
{
    public class MessageReceiverFactory : IMessageReceiverFactory
    {
        private string _connectionString;

        public MessageReceiverFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AzureCore.IMessageReceiver Receiver(string queueName)
        {
            return new AzureCore.MessageReceiver(_connectionString, queueName);
        }

        public void TimedOut()
        {
        }
    }
}
