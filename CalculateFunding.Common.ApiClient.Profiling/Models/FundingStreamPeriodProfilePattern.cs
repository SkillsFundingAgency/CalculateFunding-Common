using System;
using System.Collections.Generic;
using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class FundingStreamPeriodProfilePattern : IIdentifiable
    {
        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("roundingStrategy")]
        public RoundingStrategy RoundingStrategy { get; set; }

        [JsonProperty("fundingLineId")]
        public string FundingLineId { get; set; }
        
        [JsonProperty("profilePatternKey")]
        public string ProfilePatternKey { get; set; }

        [JsonProperty("fundingStreamPeriodStartDate")]
        public DateTime FundingStreamPeriodStartDate { get; set; }

        [JsonProperty("fundingStreamPeriodEndDate")]
        public DateTime FundingStreamPeriodEndDate { get; set; }

        [JsonProperty("allowUserToEditProfilePattern")]
        public bool AllowUserToEditProfilePattern { get; set; }

        [JsonProperty("profilePattern")]
        public ProfilePeriodPattern[] ProfilePattern { get; set; }

        [JsonProperty("profilePatternDisplayName")]
        public string ProfilePatternDisplayName { get; set; }

        [JsonProperty("profilePatternDescription")]
        public string ProfilePatternDescription { get; set; }

        [JsonProperty("providerTypes")]
        public IEnumerable<string> ProviderTypes { get; set; }

        [JsonProperty("providerSubTypes")]
        public IEnumerable<string> ProviderSubTypes { get; set; }

        [JsonProperty("profileCacheETag")]
        public string ProfileCacheETag { get; set; }

        [JsonProperty("eTag")]
        public string ETag { get; set; }

        [JsonProperty("id")]
        public string Id => $"{FundingPeriodId}-{FundingStreamId}-{FundingLineId}{ProfilePatternKeyString}";

        private string ProfilePatternKeyString => string.IsNullOrWhiteSpace(ProfilePatternKey) ? null : $"-{ProfilePatternKey}";
    }
}