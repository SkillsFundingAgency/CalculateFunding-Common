using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class EditSpecificationModel
    {
        public EditSpecificationModel()
        {
            FundingStreamIds = Enumerable.Empty<string>();
        }

        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public IEnumerable<string> FundingStreamIds { get; set; }
    }
}
