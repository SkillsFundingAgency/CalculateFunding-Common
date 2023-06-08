using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class FDSTableDefinitions
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fieldDefinitions")]
        public List<FDSFieldDefinition> FDSFieldDefinitions { get; set; }


    }
}