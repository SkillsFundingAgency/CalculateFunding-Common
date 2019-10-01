using System;
using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class Calculation : Reference
    {
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("current")]
        public CalculationVersion Current { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        public CalculationSpecificationType CalculationType { get; set; }

        public Reference AllocationLine { get; set; }

        public string Description { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}