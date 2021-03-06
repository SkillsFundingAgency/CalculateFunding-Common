﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    [Obsolete]
    public class FundingStructure
    {
        [JsonProperty("items")]
        public IEnumerable<FundingStructureItem> Items { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTimeOffset LastModified { get; set; }
    }
}
