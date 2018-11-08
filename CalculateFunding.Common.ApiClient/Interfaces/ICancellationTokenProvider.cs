using System.Threading;

namespace CalculateFunding.Common.ApiClient.Interfaces
{
    public interface ICancellationTokenProvider
    {
        CancellationToken CurrentCancellationToken();
    }
}
