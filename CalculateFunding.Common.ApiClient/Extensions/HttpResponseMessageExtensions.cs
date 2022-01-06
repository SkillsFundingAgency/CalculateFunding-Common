using System;
using System.Net.Http;
using System.Reflection;

namespace CalculateFunding.Common.ApiClient.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static bool HasContent(this HttpResponseMessage value)
        {
            return value.Content != null && value.Content.Headers.ContentLength > 0;
        }
    }
}
