using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class UpdateFundingStructureLastModifiedRequest
    {
        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("lastModified")]
        public DateTimeOffset LastModified { get; set; }
    }
}
