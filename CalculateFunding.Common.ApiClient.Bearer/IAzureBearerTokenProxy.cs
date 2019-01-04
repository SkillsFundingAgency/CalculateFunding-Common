using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Bearer
{
    public interface IAzureBearerTokenProxy
    {
        Task<AzureBearerToken> FetchToken(AzureBearerTokenOptions azureBearerTokenOptions);
    }
}
