using System.Threading.Tasks;

namespace CalculateFunding.Common.Models.HealthCheck
{
	public interface IHealthChecker
    {
        Task<ServiceHealth> IsHealthOk();
    }
}
