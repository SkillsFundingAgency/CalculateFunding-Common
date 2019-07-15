using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// Details about the period.
    /// </summary>
    public class FundingPeriod
    {
        /// <summary>
        /// The code for the period (e.g. AY1920).
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// The name of the period (e.g. Academic Year 2019-20). 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the period (academic or financial year).
        /// </summary>
        [EnumDataType(typeof(PeriodType))]
        [JsonProperty("type")]
        public PeriodType Type { get; set; }

        /// <summary>
        /// The start date for the period.
        /// </summary>
        [JsonProperty("startDate")]
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// The end date for the period.
        /// </summary>
        [JsonProperty("endDate")]
        public DateTimeOffset EndDate { get; set; }
    }
}