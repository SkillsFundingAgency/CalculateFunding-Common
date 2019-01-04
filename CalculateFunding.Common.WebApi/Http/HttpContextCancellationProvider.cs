using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CalculateFunding.Common.WebApi.Http
{
    public class HttpContextCancellationProvider : ICancellationTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCancellationProvider(IHttpContextAccessor httpContextAccessor)
        {
            Guard.ArgumentNotNull(httpContextAccessor, nameof(httpContextAccessor));

            _httpContextAccessor = httpContextAccessor;
        }

        public CancellationToken CurrentCancellationToken()
        {
            return _httpContextAccessor.HttpContext.RequestAborted;
        }
    }
}
