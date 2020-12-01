using System;
using System.Collections.Generic;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    [Obsolete("This class is legacy")]
    public class SpecificationVersion : VersionedItem
    {
        [JsonProperty("id")]
        public override string Id => $"{SpecificationId}_version_{Version}";

        [JsonProperty("entityId")]
        public override string EntityId => $"{SpecificationId}";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("fundingPeriod")]
        public Reference FundingPeriod { get; set; }

        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }

        [JsonProperty("fundingStreams")]
        public IEnumerable<Reference> FundingStreams { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("dataDefinitionRelationshipIds")]
        public IEnumerable<string> DataDefinitionRelationshipIds { get; set; }

        [JsonProperty("variationDate")]
        public DateTimeOffset? VariationDate { get; set; }

        [JsonProperty("templateId")]
        public string TemplateId { get; set; }

        [JsonProperty("templateIds")]
        public Dictionary<string, string> TemplateIds { get; set; } = new Dictionary<string, string>();
        
        [JsonProperty("profileVariationPointers")]
        public IEnumerable<ProfileVariationPointer> ProfileVariationPointers { get; set; }

        [JsonProperty("providerSource")]
        public ProviderSource ProviderSource { get; set; }

        [JsonProperty("providerSnapshotId")]
        public int? ProviderSnapshotId { get; set; }

        [JsonProperty("coreProviderVersionUpdates")]
        public CoreProviderVersionUpdates CoreProviderVersionUpdates { get; set; }

        public void AddOrUpdateTemplateId(string fundingStreamId,
            string templateId)
        {
            if (TemplateIds.ContainsKey(fundingStreamId))
                TemplateIds[fundingStreamId] = templateId;
            else
                TemplateIds.Add(fundingStreamId, templateId);
        }

        public override VersionedItem Clone()
        {
            // Serialise to perform a deep copy
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<SpecificationVersion>(json);
        }
    }
}
