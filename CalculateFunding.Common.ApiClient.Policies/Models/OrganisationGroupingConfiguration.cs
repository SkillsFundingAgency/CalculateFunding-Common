using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class OrganisationGroupingConfiguration
    {
        public OrganisationGroupTypeIdentifier GroupTypeIdentifier { get; set; }

        public GroupingReason GroupingReason { get; set; }

        public OrganisationGroupTypeClassification GroupTypeClassification { get; set; }

        public OrganisationGroupTypeCode OrganisationGroupTypeCode { get; set; }

        public IEnumerable<ProviderTypeMatch> ProviderTypeMatch { get; set; }
    }
}
