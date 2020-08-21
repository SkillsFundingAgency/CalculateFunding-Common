using System;

namespace CalculateFunding.Common.JobManagement
{
    public class JobNotFoundException : Exception
    {
        public JobNotFoundException(string message, string jobId) 
            : base(message)
        {
            JobId = jobId;
        }

        public JobNotFoundException(string message, Exception innerException, string jobId) 
            : base(message, innerException)
        {
            JobId = jobId;
        }

        public string JobId { get; }
    }
}
