using CalculateFunding.Common.Interfaces;
using System.Threading;

namespace CalculateFunding.Common.ApiClient
{
    public class InactiveCancellationTokenProvider : ICancellationTokenProvider
    {
        public CancellationToken CurrentCancellationToken()
        {
            return default(CancellationToken);
        }
    }
}
