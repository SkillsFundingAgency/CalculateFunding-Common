using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class SpecificationDatasourceRelationshipSearchResultItem
    {
        [JsonProperty("specificationId")]
        public string Id { get; set; }

        [JsonProperty("specificationName")]
        public string Name { get; set; }

        [JsonProperty("definitionRelationshipCount")]
        public int RelationshipCount { get; set; }
        
        [JsonProperty("fundingStreamNames")]
        public string[] FundingStreamNames { get; set; }
        
        [JsonProperty("fundingPeriodName")]
        public string FundingPeriodName { get; set; }

        [JsonProperty("converterEnabled")]
        public bool ConverterEnabled { get; set; }

        [JsonProperty("converterEligible")]
        public bool ConverterEligible { get; set; }

        [JsonProperty("mapDatasetLastUpdated")]
        public DateTimeOffset? MapDatasetLastUpdated { get; set; }
        
        [JsonProperty("totalMappedDataSets")]
        public int TotalMappedDataSets { get; set; }
    }
}
