using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProviderSearchItem
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("providerType")]
        public string ProviderType { get; set; }
        
        [JsonProperty("providerSubType")]
        public string ProviderSubType { get; set; }

        [JsonProperty("localAuthority")]
        public string LocalAuthority { get; set; }

        [JsonProperty("fundingStatus")]
        public string FundingStatus { get; set; }

        [JsonProperty("providerName")]
        public string ProviderName { get; set; }

        [JsonProperty("ukprn")]
        public string UKPRN { get; set; }
        
        [JsonProperty("upin")]
        public string UPIN { get; set; }
        
        [JsonProperty("urn")]
        public string URN { get; set; }

        [JsonProperty("fundingValue")]
        public double FundingValue { get; set; }

        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }
        
        [JsonProperty("indicative")]
        public string Indicative { get; set; }

        [JsonProperty("isIndicative")]
        public bool IsIndicative { get; set; }

        [JsonProperty("hasErrors")]
        public bool HasErrors { get; set; }
        
        [JsonProperty("errors")]
        public string[] Errors { get; set; }
        
        [JsonProperty("dateOpened")]
        public DateTimeOffset? DateOpened { get; set; }
    }
}
