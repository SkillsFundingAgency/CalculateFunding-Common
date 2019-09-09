using CalculateFunding.Common.ApiClient.Policies.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Generators.OrganisationGroup
{
    public class OrganisationGroupLookupParameters
    {
        public string identifierValue { get; set; }
        public OrganisationGroupTypeCode organisationGroupTypeCode { get; set; }
        public string providerVersionId { get; set; }
        public OrganisationGroupTypeIdentifier groupTypeIdentifier { get; set; }
    }
}
