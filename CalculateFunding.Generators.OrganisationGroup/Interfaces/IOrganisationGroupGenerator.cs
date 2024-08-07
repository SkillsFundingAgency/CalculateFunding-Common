﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Generators.OrganisationGroup.Models;

namespace CalculateFunding.Generators.OrganisationGroup.Interfaces
{
    public interface IOrganisationGroupGenerator
    {
        Task<IEnumerable<OrganisationGroupResult>> GenerateOrganisationGroup(
            FundingConfiguration fundingConfiguration,
            IEnumerable<Provider> scopedProviders,
            string providerVersionId,
            int? providerSnapshotId = null);

        Task<IEnumerable<OrganisationGroupResult>> GenerateOrganisationGroup(
            IEnumerable<OrganisationGroupingConfiguration> organisationGroupingConfigurations,
            ProviderSource providerSource,
            PaymentOrganisationSource paymentOrganisationSource,
            IEnumerable<Provider> scopedProviders,
            string providerVersionId,
            int? providerSnapshotId = null);

        Task<IEnumerable<string>> GetGroupTypeIdentifierList(FundingConfiguration fundingConfiguration);
    }
}
