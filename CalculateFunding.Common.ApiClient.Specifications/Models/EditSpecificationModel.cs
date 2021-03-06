﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class EditSpecificationModel
    {
        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }

        [JsonProperty("providerSnapshotId")]
        public int? ProviderSnapshotId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("assignedTemplateIds")]
        public IDictionary<string, string> AssignedTemplateIds { get; set; }

        [JsonProperty("coreProviderVersionUpdates")]
        public CoreProviderVersionUpdates CoreProviderVersionUpdates { get; set; }
    }
}