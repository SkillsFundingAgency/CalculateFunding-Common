namespace CalculateFunding.Common.WebApi.Models
{
	using System.Collections.Generic;

	public class OverallHealth
    {
        public bool OverallHealthOk { get; set; }

        public ICollection<ServiceHealth> Services { get; } = new List<ServiceHealth>();
    }
}
