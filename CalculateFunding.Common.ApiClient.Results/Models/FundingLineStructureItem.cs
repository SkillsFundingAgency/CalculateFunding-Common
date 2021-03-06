﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    [Obsolete]
    public class FundingStructureItem
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fundingLineCode")]
        public string FundingLineCode { get; set; }

        [JsonProperty("calculationId")]
        public string CalculationId { get; set; }

        [JsonProperty("calculationPublishStatus")]
        public string CalculationPublishStatus { get; set; }

        [JsonProperty("type")]
        public FundingStructureType Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("calculationType")]
        public string CalculationType { get; set; }

        [JsonProperty("fundingStructureItems")]
        public ICollection<FundingStructureItem> FundingStructureItems { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}
