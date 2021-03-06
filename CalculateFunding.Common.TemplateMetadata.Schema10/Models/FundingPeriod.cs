﻿using System;
using System.ComponentModel.DataAnnotations;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// Details about the period.
    /// </summary>
    public class FundingPeriod
    {
        /// <summary>
        /// Funding Period ID eg AY-2021
        /// </summary>
        [JsonProperty("id")]
        public string Id => $"{Type}-{Period}";

        /// <summary> 
        /// The code for the period (e.g. 1920 or 2021).
        /// </summary>
        [JsonProperty("period")]
        public string Period { get; set; }

        /// <summary>
        /// The name of the period (e.g. Academic Year 2019-20). 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The type of the period (AY or FY).
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