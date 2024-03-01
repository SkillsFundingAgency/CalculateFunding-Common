using CalculateFunding.Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class ObsoleteCalculationResponseModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("calculationType")]
        public string CalculationType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("valueType")]
        public string ValueType { get; set; }

        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonProperty("lastUpdated")]
        public DateTimeOffset? LastUpdated { get; set; }

        [JsonProperty("author")]
        public Reference Author { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

    }
}
