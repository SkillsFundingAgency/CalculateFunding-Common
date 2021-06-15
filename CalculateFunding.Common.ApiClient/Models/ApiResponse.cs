using System.Net.Http.Headers;
using System.Net;

namespace CalculateFunding.Common.ApiClient.Models
{
    public class ApiResponse<T>
    {
        public ApiResponse(HttpStatusCode statusCode,
            HttpResponseHeaders headers,
            T content = default,
            string message = null)
        : this(statusCode, content)
        {
            Headers = headers;
            Message = message;
        }
        
        public ApiResponse(HttpStatusCode statusCode,
            T content = default,
            string message = null)
        {
            StatusCode = statusCode;
            Content = content;
            Message = message;
        }

        public HttpStatusCode StatusCode { get; private set; }
        
        public HttpResponseHeaders Headers { get; private set; }

        public string Message { get; private set; }

        public T Content { get; private set; }
    }
}