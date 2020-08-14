using System.Net.Http.Headers;
using System.Net;

namespace CalculateFunding.Common.ApiClient.Models
{
    public class ApiResponse<T>
    {
        public ApiResponse(HttpStatusCode statusCode,
            HttpResponseHeaders headers,
            T content = default)
        : this(statusCode, content)
        {
            Headers = headers;
        }
        
        public ApiResponse(HttpStatusCode statusCode, T content = default)
        {
            StatusCode = statusCode;
            Content = content;
        }

        public HttpStatusCode StatusCode { get; private set; }
        
        public HttpResponseHeaders Headers { get; private set; }

        public T Content { get; private set; }
    }
}