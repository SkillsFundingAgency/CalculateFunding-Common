using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Bearer
{
    public interface IBearerTokenProvider
    {
        Task<string> GetToken();

        Task<(bool Ok, string Message)> IsHealthOk();
    }
}
