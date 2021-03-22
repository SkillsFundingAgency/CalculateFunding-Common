using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.JobManagement
{
    public class JobsNotRetrievedException : Exception
    {
        public JobsNotRetrievedException(
            string message, 
            string specificationId,
            IEnumerable<string> jobDefinitionIds)
            : base(message)
        {
            JobDefinitionIds = jobDefinitionIds.ToArray();
            SpecificationId = specificationId;
        }

        public string SpecificationId { get; }

        public IEnumerable<string> JobDefinitionIds { get; }
    }
}
