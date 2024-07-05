using System;
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
        
        /// <summary>
        /// Flag indicating whether the re profiling
        /// should use a new opener or new opener with catch up if blank then new allocation strategy 
        /// </summary>
        [JsonProperty("midYearType")]
        public MidYearType? MidYearType { get; set; }

        /// <summary>
        /// Date Opened of a published provider
        /// </summary>
        [JsonProperty("dateOpened")]
        public DateTimeOffset? DateOpened { get; set; }

        /// <summary>
        /// Flag to identify if the request is from ReprofileOnDemand
        /// </summary>
        [JsonProperty("isReProfileOnDemandTriggered")]
        public bool IsReProfileOnDemandTriggered { get; set; }

        /// <summary>
        /// Flag to identify if the fundingline is ReprofiledOnDemand previously
        /// </summary>
        [JsonProperty("hasPreviouslyReProfiledOnDemand")]
        public bool HasPreviouslyReProfiledOnDemand { get; set; }

        /// <summary>
        /// The index into the ordered refresh profile periods
        /// to start paying from
        /// </summary>
        [JsonProperty("variationPointerIndex")]
        public int? VariationPointerIndex { get; set; }
    }
}