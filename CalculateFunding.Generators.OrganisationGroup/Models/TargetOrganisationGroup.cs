using System.Collections.Generic;

namespace CalculateFunding.Generators.OrganisationGroup.Models
{
    public class TargetOrganisationGroup
    {
        public string Identifier { get; set; }

        public string Name { get; set; }
        public IEnumerable<OrganisationIdentifier> Identifiers { get; set; }
    }
}
