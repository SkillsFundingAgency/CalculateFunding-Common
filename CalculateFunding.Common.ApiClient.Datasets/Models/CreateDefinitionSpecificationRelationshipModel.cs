using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class CreateDefinitionSpecificationRelationshipModel
    {
        public string DatasetDefinitionId { get; set; }

        public string SpecificationId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSetAsProviderData { get; set; }

        public bool UsedInDataAggregations { get; set; }

        public bool ConverterEnabled { get; set; }

        public DatasetRelationshipType RelationshipType { get; set; }

        public string TargetSpecificationId { get; set; }

        public IEnumerable<uint> FundingLineIds { get; set; }

        public IEnumerable<uint> CalculationIds { get; set; }

        public string FundingPeriodName { get; set; }   
    }
}