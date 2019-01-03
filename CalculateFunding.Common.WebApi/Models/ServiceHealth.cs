namespace CalculateFunding.Common.WebApi.Models
{
	using System.Collections.Generic;

	public class ServiceHealth
    {
        public string Name { get; set; }

        public ICollection<DependencyHealth> Dependencies { get; } = new List<DependencyHealth>();
    }
}
