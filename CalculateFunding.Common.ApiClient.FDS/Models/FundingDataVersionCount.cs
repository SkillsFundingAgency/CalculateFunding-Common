using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class FundingDataVersionCount
    {
        [JsonProperty("schemaName")]
        public string SchemaName { get; set; }

        [JsonProperty("fundingDataSchemaId")]
        public int SchemaId { get; set; }

        [JsonProperty("fundingDataSchemaVersionCount")]
        public int FundingDataSchemaVersionCount { get; set; }
    }
}
