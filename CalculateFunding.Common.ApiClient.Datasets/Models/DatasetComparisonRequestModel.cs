using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetComparisonRequestModel
    {
        [JsonProperty("fundingStreamCode")]
        public string FundingStreamCode { get; set; }

        [JsonProperty("fundingPeriodCode")]
        public string FundingPeriodCode { get; set; }

        [JsonProperty("schemaName")]
        public string SchemaName { get; set; }
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }
        [JsonProperty("relationshipId")]
        public string RelationshipId { get; set; }
        [JsonProperty("includeAffectedCalcs")]
        public bool IncludeAffectedCalcs { get; set; }
        [JsonProperty("removedFields")]
        public List<DatasetComparisonField> RemovedFields { get; set; }
    }
}
