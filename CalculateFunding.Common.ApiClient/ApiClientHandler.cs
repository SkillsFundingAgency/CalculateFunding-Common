using System.Net;
using System.Net.Http;

namespace CalculateFunding.Common.ApiClient
{
    public class ApiClientHandler : HttpClientHandler
    {
        public ApiClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }
    }
}
