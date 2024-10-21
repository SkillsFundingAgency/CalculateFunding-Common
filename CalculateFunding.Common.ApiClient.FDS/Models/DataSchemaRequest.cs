using Newtonsoft.Json;

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
