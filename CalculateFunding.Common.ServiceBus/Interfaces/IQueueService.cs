using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ServiceBus.Interfaces
{
    public interface IQueueService
    {
        Task CreateQueue(string entityPath);

        Task DeleteQueue(string entityPath);
    }
}
