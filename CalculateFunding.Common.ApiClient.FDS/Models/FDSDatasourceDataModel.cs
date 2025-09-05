using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class FDSDatasourceDataModel
    {
        [JsonProperty("snapshotId")]
        public int SnapshotId { get; set; }

        [JsonProperty("fundingStreamCode")]
        public string FundingStreamCode { get; set; }

        [JsonProperty("fundingStreamName")]
        public string FundingStreamName { get; set; }

        [JsonProperty("fundingPeriodCode")]
        public string FundingPeriodCode { get; set; }

        [JsonProperty("fundingPeriodName")]
        public string FundingPeriodName { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("createdDt")]
        public DateTime CreatedDt { get; set; }

        [JsonProperty("publishStatus")]
        public string PublishStatus { get; set; }

        [JsonProperty("rows")]
        public List<Row> Rows { get; set; }

        [JsonProperty("dataGranularity")]
        public string DataGranularity { get; set; }
    }
}
