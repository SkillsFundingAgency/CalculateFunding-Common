using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig
{
    public class FundingConfiguration
    {
        public FundingConfiguration()
        {
            OrganisationGroupings = Enumerable.Empty<OrganisationGroupingConfiguration>();

        }

        public IEnumerable<OrganisationGroupingConfiguration> OrganisationGroupings { get; set; }
    }
}
