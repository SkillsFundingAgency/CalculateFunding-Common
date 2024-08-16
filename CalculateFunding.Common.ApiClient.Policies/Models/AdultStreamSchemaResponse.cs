using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class AdultStreamSchemaResponse
    {
        [JsonProperty("fundingStreamCode")]
        public string FundingStreamCode { get; set; }

        [JsonProperty("schemaNames")]
        public List<string> SchemaNames { get; set; }
    }
}
