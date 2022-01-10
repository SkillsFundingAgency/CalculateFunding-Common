using System;
using System.Net.Http;
using System.Reflection;

namespace CalculateFunding.Common.ApiClient.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static bool HasContent(this HttpContent httpContent)
        {
            if (httpContent == null)
            {
                return false;
            }
            var stream = httpContent.ReadAsStreamAsync().Result;
            return stream.ReadByte() != -1;
        }
    }
}
