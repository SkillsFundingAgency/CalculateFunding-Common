using System;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class PermissionOptions
    {
        public Guid AdminGroupId { get; set; }

        public string HttpClientName { get; set; }
    }
}
