using System.Net;
using System.Net.Http;

namespace CalculateFunding.Frontend.Clients
{
    public class ApiClientHandler : HttpClientHandler
    {
        public ApiClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        }
    }
}
