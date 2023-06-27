using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{

    public class Schema
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("fundingStreamId")]
        public int FundingStreamId { get; set; }

        [JsonProperty("fundingPeriodId")]
        public int FundingPeriodId { get; set; }

        [JsonProperty("comment")]
        public object Comment { get; set; }

        [JsonProperty("createdDate")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("createdBy")]
        public string CreatedBy { get; set; }

        [JsonProperty("updatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }

        [JsonProperty("fundingDataDefinitions")]
        public List<FDSFieldDefinition> FundingDataDefinitions { get; set; }

        [JsonProperty("fundingDataSnapshotHistory")]
        public List<object> FundingDataSnapshotHistory { get; set; }

        [JsonProperty("fundingDataStagingTransaction")]
        public List<object> FundingDataStagingTransaction { get; set; }
    }

}
