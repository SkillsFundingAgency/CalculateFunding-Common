namespace CalculateFunding.Common.WebApi
{
	using System.Threading.Tasks;
	using Models;

	public interface IHealthChecker
    {
        Task<ServiceHealth> IsHealthOk();
    }
}
