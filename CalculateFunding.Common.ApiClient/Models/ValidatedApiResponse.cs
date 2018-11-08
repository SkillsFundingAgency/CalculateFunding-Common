namespace CalculateFunding.Common.ApiClient.Models
{
    using System.Collections.Generic;
    using System.Net;

    public class ValidatedApiResponse<T> : ApiResponse<T>
    {
        public ValidatedApiResponse(HttpStatusCode statusCode, T content = default(T))
            : base(statusCode, content)
        {
        }

        public IDictionary<string, IEnumerable<string>> ModelState { get; set; }
    }
}