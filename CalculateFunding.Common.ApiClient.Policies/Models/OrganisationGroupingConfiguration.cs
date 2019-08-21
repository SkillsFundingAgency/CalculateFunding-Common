using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class OrganisationGroupingConfiguration
    {
        public OrganisationIdentifierType IdentifierType { get; set; }

        public GroupingReason GroupingReason { get; set; }

        public OrganisationGroupingType OrganisationGroupingType { get; set; }

        public IEnumerable<ProviderTypeMatch> ProviderTypeMatch { get; set; }
    }
}
