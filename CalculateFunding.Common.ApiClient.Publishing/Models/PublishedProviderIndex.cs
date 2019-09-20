using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProviderIndex
    {
        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("providerType")]
        public string ProviderType { get; set; }
       
        [JsonProperty("localAuthority")]
        public string LocalAuthority { get; set; }
       
        [JsonProperty("fundingStatus")]
        public string FundingStatus { get; set; }
       
        [JsonProperty("providerName")]
        public string ProviderName { get; set; }
        
        [JsonProperty("ukprn")]
        public string UKPRN { get; set; }
        
        [JsonProperty("fundingValue")]
        public double FundingValue { get; set; }
        
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }
        
        [JsonProperty("fundingStreamIds")]
        public string[] FundingStreamIds { get; set; }
        
        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }    }
}
