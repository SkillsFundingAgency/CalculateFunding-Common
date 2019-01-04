using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Bearer
{
    public interface IBearerTokenProvider
    {
        Task<string> GetToken();
    }
}
