using System.Collections.Generic;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class ProviderSourceDatasetVersion : VersionedItem
    {
        [JsonProperty("id")]
        public override string Id => $"{ProviderSourceDatasetId}_version_{Version}";

        [JsonProperty("entityId")]
        public override string EntityId => ProviderSourceDatasetId;

        [JsonProperty("providerSourceDatasetId")]
        public string ProviderSourceDatasetId { get; set; }

        [JsonProperty("dataset")]
        public VersionReference Dataset { get; set; }

        [JsonProperty("rows")]
        public List<Dictionary<string, object>> Rows { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        [JsonProperty("providerId")]
        public string ProviderId { get; set; }
        
        public override VersionedItem Clone()
        {
            return JsonConvert.DeserializeObject<ProviderSourceDatasetVersion>(JsonConvert.SerializeObject(this));
        }
    }
}