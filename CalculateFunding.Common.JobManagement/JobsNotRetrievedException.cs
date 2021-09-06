using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.JobManagement
{
    public class JobsNotRetrievedException : Exception
    {
        public JobsNotRetrievedException(
            string message, 
            IEnumerable<string> jobDefinitionIds,
            string specificationId = null)
            : base(message)
        {
            JobDefinitionIds = jobDefinitionIds.ToArray();
            SpecificationId = specificationId;
        }

        public string SpecificationId { get; }

        public IEnumerable<string> JobDefinitionIds { get; }
    }
}
