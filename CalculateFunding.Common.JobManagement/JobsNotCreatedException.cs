using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.JobManagement
{
    public class JobsNotCreatedException : Exception
    {
        public JobsNotCreatedException(string message, IEnumerable<string> jobDefinitionIds) 
            : base(message)
        {
            JobDefinitionIds = jobDefinitionIds.ToArray();
        }

        public IEnumerable<string> JobDefinitionIds { get; }
    }
}