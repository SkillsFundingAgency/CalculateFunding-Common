using System.Collections.Generic;

namespace CalculateFunding.Common.Models.HealthCheck
{
	public class OverallHealth
    {
        public bool OverallHealthOk { get; set; }

        public ICollection<ServiceHealth> Services { get; } = new List<ServiceHealth>();
    }
}
