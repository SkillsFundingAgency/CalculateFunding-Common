using CalculateFunding.Common.ApiClient.Jobs.Models;
using System;

namespace CalculateFunding.Common.JobManagement
{
    public class JobAlreadyCompletedException : Exception
    {
        public JobViewModel Job { get; }

        public JobAlreadyCompletedException(string message, JobViewModel job) : base(message)
        {
            Job = job;
        }

        public JobAlreadyCompletedException(string message, Exception innerException, JobViewModel job) : base(message, innerException)
        {
            Job = job;
        }
    }
}
