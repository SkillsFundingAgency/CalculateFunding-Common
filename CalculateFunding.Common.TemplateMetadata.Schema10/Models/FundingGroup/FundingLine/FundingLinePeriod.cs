using Newtonsoft.Json;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using System.ComponentModel.DataAnnotations;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// A funding line period (e.g. the 1st March payment in 2019), with relevant value data.
    /// </summary>
    public class FundingLinePeriod
    {
        /// <summary>
        /// The type of the period (e.g. CalendarMonth).
        /// </summary>
        [EnumDataType(typeof(FundingLinePeriodType))]
        [JsonProperty("type")]
        public FundingLinePeriodType Type { get; set; }

        /// <summary>
        /// The value identifier for this period (e.g. if type is 'Calendar Month', this could be 'April').
        /// </summary>
        [JsonProperty("typeValue")]
        public string TypeValue { get; set; }

        /// <summary>
        /// Which year is the period in.
        /// </summary>
        [JsonProperty("year")]
        public int Year { get; set; }

        /// <summary>
        /// Which occurance this month (note that this is 1 indexed).
        /// </summary>
        [JsonProperty("occurence")]
        public int Occurence { get; set; }

        /// <summary>
        /// The amount of the profiled value, in pence.
        /// </summary>
        [JsonProperty("profiledValue")]
        public long ProfiledValue { get; set; }

        /// <summary>
        /// The code for the period.
        /// </summary>
        [JsonProperty("periodCode")]
        public string PeriodCode { get; set; }
    }
}