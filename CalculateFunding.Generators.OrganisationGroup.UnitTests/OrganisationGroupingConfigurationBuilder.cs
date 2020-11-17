using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Generators.OrganisationGroup.UnitTests
{
    public class OrganisationGroupingConfigurationBuilder : TestEntityBuilder
    {
        OrganisationGroupingConfiguration _config = new OrganisationGroupingConfiguration();

        public OrganisationGroupingConfiguration Build()
        {
            return _config;
        }

        public OrganisationGroupingConfigurationBuilder WithGroupingReason(GroupingReason groupingReason)
        {
            _config.GroupingReason = groupingReason;

            return this;
        }

        public OrganisationGroupingConfigurationBuilder WithOrganisationGroupTypeClassification(OrganisationGroupTypeClassification classification)
        {
            _config.GroupTypeClassification = classification;

            return this;
        }


        public OrganisationGroupingConfigurationBuilder WithGroupTypeIdentifier(OrganisationGroupTypeIdentifier groupIdentifier)
        {
            _config.GroupTypeIdentifier = groupIdentifier;

            return this;
        }


        public OrganisationGroupingConfigurationBuilder WithOrganisationGroupTypeCode(OrganisationGroupTypeCode groupTypeCode)
        {
            _config.OrganisationGroupTypeCode = groupTypeCode;

            return this;
        }

        public OrganisationGroupingConfigurationBuilder WithProviderTypeMatch(string providerType, string providerSubtype)
        {
            if (_config.ProviderTypeMatch == null)
            {
                _config.ProviderTypeMatch = new List<ProviderTypeMatch>();
            }

            _config.ProviderTypeMatch = _config.ProviderTypeMatch.Concat(new ProviderTypeMatch[] {  new ProviderTypeMatch()
                {
                    ProviderType = providerType,
                    ProviderSubtype = providerSubtype,
                }
            });

            return this;
        }

    }
}
