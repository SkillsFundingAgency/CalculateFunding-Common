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

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fundingLineCode", NullValueHandling = NullValueHandling.Ignore)]
        public string FundingLineCode { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [EnumDataType(typeof(FundingLineType))]
        [JsonProperty("type")]
        public FundingLineType Type { get; set; }

        [JsonProperty("calculations")]
        public IEnumerable<Calculation> Calculations { get; set; }

        [JsonProperty("fundingLines")]
        public IEnumerable<FundingLine> FundingLines { get; set; }
    }
}