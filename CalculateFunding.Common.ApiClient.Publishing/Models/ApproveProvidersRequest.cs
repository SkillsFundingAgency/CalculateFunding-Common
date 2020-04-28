using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ApproveProvidersRequest
    {
        public IEnumerable<string> Providers { get; set; }
    }
}
