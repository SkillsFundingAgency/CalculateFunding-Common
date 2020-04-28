using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishProvidersRequest
    {
        public IEnumerable<string> Providers { get; set; }
    }
}
