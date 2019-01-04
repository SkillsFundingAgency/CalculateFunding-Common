using System.Threading;

namespace CalculateFunding.Common.Interfaces
{
    public interface ICancellationTokenProvider
    {
        CancellationToken CurrentCancellationToken();
    }
}
