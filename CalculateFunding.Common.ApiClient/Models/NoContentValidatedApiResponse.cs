namespace CalculateFunding.Common.ApiClient.Models
{
    using System.Collections.Generic;
    using System.Net;

    public class NoValidatedContentApiResponse
    {
        public NoValidatedContentApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public IDictionary<string, IEnumerable<string>> ModelState { get; set; }
    }
}