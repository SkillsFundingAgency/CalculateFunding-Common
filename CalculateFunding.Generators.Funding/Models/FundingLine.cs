using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CalculateFunding.Generators.Funding.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Generators.Funding.Models
{
    public class FundingLine
    {
        public FundingLine()
        {
        }

        /// <summary>
        /// The name of a funding line (e.g. "Total funding line").
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// External FundingLine Name - Optional field used for communicate funding team.
        /// </summary>
        [JsonProperty("externalFundingLineName")]
        public string ExternalFundingLineName { get; set; }

        /// <summary>
        /// Funding Line Code - unique code within the template to lookup this specific funding line.
        /// Used to map this funding line in consuming systems (eg nav for payment)
        /// </summary>
        [JsonProperty("fundingLineCode", NullValueHandling = NullValueHandling.Ignore)]
        public string FundingLineCode { get; set; }

        /// <summary>
        /// A unique ID (in terms of template, not data) for this funding line (e.g. 345).
        /// </summary>
        [JsonProperty("TemplateLineId")]
        public uint TemplateLineId { get; set; }

        /// <summary>
        /// The funding value in pounds.pence
        /// </summary>
        [JsonProperty("value")]
        public decimal? Value { get; set; }

        [EnumDataType(typeof(FundingLineType))]
        [JsonProperty("type")]
        public FundingLineType Type { get; set; }

        [JsonProperty("calculations")]
        public IEnumerable<Calculation> Calculations { get; set; }

        [JsonProperty("fundingLines")]
        public IEnumerable<FundingLine> FundingLines { get; set; }

        /// <summary>
        /// Profile periods for this funding line
        /// </summary>
        [JsonProperty("profilePeriods")]
        public IEnumerable<DistributionPeriod> DistributionPeriods { get; set; }
    }
}