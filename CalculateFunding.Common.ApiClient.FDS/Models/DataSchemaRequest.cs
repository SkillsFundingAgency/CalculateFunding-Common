using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class DataSchemaRequest
    {
        [JsonProperty("fundingStreamCode")]
        public string FundingStreamCode { get; set; }

        [JsonProperty("fundingPeriodCode")]
        public string FundingPeriodCode { get; set; }

        [JsonProperty("schemaName")]
        public string SchemaName { get; set; }

    }
}
