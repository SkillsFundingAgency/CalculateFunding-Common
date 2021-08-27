using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ReProfileRequest
    {
        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("fundingLineCode")]
        public string FundingLineCode { get; set; }

        /// <summary>
        /// Profile pattern key - null or empty string for default pattern or a valid profile pattern key for this funding stream/period/line.
        /// </summary>
        [JsonProperty("profilePatternKey")]
        public string ProfilePatternKey { get; set; }

        [JsonProperty("existingFundingLineTotal")]
        public decimal ExistingFundingLineTotal { get; set; }

        /// <summary>
        /// The current / target funding line total
        /// </summary>
        [JsonProperty("fundingLineTotal")]
        public decimal FundingLineTotal { get; set; }

        /// <summary>
        /// Profile periods which have already been paid
        /// </summary>
        [JsonProperty("existingPeriods")]
        public IEnumerable<ExistingProfilePeriod> ExistingPeriods { get; set; }

        [JsonProperty("configurationType")]
        public ProfileConfigurationType ConfigurationType { get; set; }
        
        /// <summary>
        /// Flag indicating whether the re profiling
        /// should use a new opener or new opener with catch up if blank then new allocation strategy 
        /// </summary>
        [JsonProperty("midYearCatchup")]
        public bool? MidYearCatchup { get; set; }
        
        /// <summary>
        /// The index into the ordered refresh profile periods
        /// to start paying from
        /// </summary>
        [JsonProperty("variationPointerIndex")]
        public int? VariationPointerIndex { get; set; }

    }
}