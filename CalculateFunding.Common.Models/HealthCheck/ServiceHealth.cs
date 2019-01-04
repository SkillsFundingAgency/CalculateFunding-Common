using System.Collections.Generic;

namespace CalculateFunding.Common.Models.HealthCheck
{
	public class ServiceHealth
    {
        public string Name { get; set; }

        public ICollection<DependencyHealth> Dependencies { get; } = new List<DependencyHealth>();
    }
}
