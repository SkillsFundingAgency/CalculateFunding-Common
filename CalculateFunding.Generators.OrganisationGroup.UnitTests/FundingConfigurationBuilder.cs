using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Generators.OrganisationGroup.UnitTests
{
    public class FundingConfigurationBuilder : TestEntityBuilder
    {

        private readonly FundingConfiguration _config = new FundingConfiguration();

        public FundingConfigurationBuilder WithPaymentOrganisationSource(PaymentOrganisationSource paymentOrganisationSource)
        {
            _config.PaymentOrganisationSource = paymentOrganisationSource;

            return this;
        }

        public FundingConfigurationBuilder WithOrganisationGroup(OrganisationGroupingConfiguration organisationGroupingConfiguration)
        {
            if (_config.OrganisationGroupings == null)
            {
                _config.OrganisationGroupings = new List<OrganisationGroupingConfiguration>();
            }

            _config.OrganisationGroupings = _config.OrganisationGroupings.Concat(new OrganisationGroupingConfiguration[] { organisationGroupingConfiguration });

            return this;
        }

        public FundingConfigurationBuilder WithFundingStreamId(string fundingStreamId)
        {
            _config.FundingStreamId = fundingStreamId;
            return this;
        }

        public FundingConfigurationBuilder WithProviderSource(ProviderSource providerSource)
        {
            _config.ProviderSource = providerSource;
            return this;
        }

        public FundingConfiguration Build()
        {
            return _config;
        }
    }
}
