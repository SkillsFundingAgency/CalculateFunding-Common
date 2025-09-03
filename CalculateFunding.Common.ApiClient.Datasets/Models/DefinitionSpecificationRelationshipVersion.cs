using CalculateFunding.Common.Models;
using System;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DefinitionSpecificationRelationshipVersion
    {
        public string RelationshipId { get; set; }
        public string Name { get; set; }
        public Reference DatasetDefinition { get; set; }
        public Reference Specification { get; set; }
        public string Description { get; set; }
        public DatasetRelationshipVersion DatasetVersion { get; set; }
        public bool IsSetAsProviderData { get; set; }
        public bool ConverterEnabled { get; set; }
        public bool UsedInDataAggregations { get; set; }
        public DateTimeOffset? LastUpdated { get; set; }
        public DatasetRelationshipType RelationshipType { get; set; }
        public PublishedSpecificationConfiguration PublishedSpecificationConfiguration { get; set; }
        public string FundingPeriodName { get; set; }
    }
}
