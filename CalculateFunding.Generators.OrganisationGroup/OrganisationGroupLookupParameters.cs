using CalculateFunding.Common.ApiClient.Policies.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Generators.OrganisationGroup
{
    public class OrganisationGroupLookupParameters
    {
        public string IdentifierValue { get; set; }
        public OrganisationGroupTypeCode? OrganisationGroupTypeCode { get; set; }
        public string ProviderVersionId { get; set; }
        public OrganisationGroupTypeIdentifier? GroupTypeIdentifier { get; set; }
    }
}
