using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Helpers;
using CalculateFunding.Common.Utility;
using CalculateFunding.Generators.OrganisationGroup.Interfaces;
using CalculateFunding.Generators.OrganisationGroup.Models;

namespace CalculateFunding.Generators.OrganisationGroup
{
    public class OrganisationGroupGenerator : IOrganisationGroupGenerator
    {
        private readonly IOrganisationGroupTargetProviderLookup _organisationGroupTargetProviderLookup;

        public OrganisationGroupGenerator(IOrganisationGroupTargetProviderLookup organisationGroupTargetProviderLookup)
        {
            _organisationGroupTargetProviderLookup = organisationGroupTargetProviderLookup;
        }

        public async Task<IEnumerable<OrganisationGroupResult>> GenerateOrganisationGroup(FundingConfiguration fundingConfiguration, IEnumerable<Provider> scopedProviders, string providerVersionId)
        {
            Guard.ArgumentNotNull(fundingConfiguration, nameof(fundingConfiguration));
            Guard.ArgumentNotNull(scopedProviders, nameof(scopedProviders));
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));

            List<OrganisationGroupResult> results = new List<OrganisationGroupResult>();

            foreach (OrganisationGroupingConfiguration grouping in fundingConfiguration.OrganisationGroupings)
            {
                // Get the provider attribute required to group
                Func<Provider, string> providerFilterAttribute = GetProviderFieldForGrouping(grouping.GroupTypeIdentifier, grouping.OrganisationGroupTypeCode, grouping.GroupingReason);

                // Filter providers based on provider type and subtypes
                IEnumerable<Provider> providersForGroup = grouping.ProviderTypeMatch.IsNullOrEmpty() ? scopedProviders : scopedProviders.Where(_ => ShouldIncludeProvider(_, grouping.ProviderTypeMatch));

                // Group providers by the fields and discard any providers with null values for that field
                IEnumerable<IGrouping<string, Provider>> groupedProviders = providersForGroup.GroupBy(providerFilterAttribute);

                // Common values for all groups
                Enums.OrganisationGroupTypeClassification organisationGroupTypeClassification = grouping.GroupingReason == GroupingReason.Payment ? Enums.OrganisationGroupTypeClassification.LegalEntity : Enums.OrganisationGroupTypeClassification.GeographicalBoundary;
                Enums.OrganisationGroupTypeCode organisationGroupTypeCode = grouping.OrganisationGroupTypeCode.AsMatchingEnum<Enums.OrganisationGroupTypeCode>();

                // Generate Organisation Group results based on the grouped providers
                foreach (IGrouping<string, Provider> providerGrouping in groupedProviders)
                {
                    // Ignore providers without the matching data in the key
                    if (string.IsNullOrWhiteSpace(providerGrouping.Key))
                    {
                        continue;
                    }

                    TargetOrganisationGroup targetOrganisationGroup = null;

                    OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
                    {
                        IdentifierValue = providerGrouping.Key,
                        OrganisationGroupTypeCode = grouping.OrganisationGroupTypeCode,
                        ProviderVersionId = providerVersionId,
                        GroupTypeIdentifier = grouping.GroupTypeIdentifier
                    };

                    targetOrganisationGroup = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters, grouping.GroupingReason, providerGrouping, fundingConfiguration.PaymentOrganisationSource);

                    if (targetOrganisationGroup == null)
                    {
                        // TODO: improve logging
                        throw new Exception($"Target Organisation Group could not be found for identifier '{providerGrouping.Key}'");
                    }

                    OrganisationGroupResult organisationGroupResult = new OrganisationGroupResult()
                    {
                        GroupTypeClassification = organisationGroupTypeClassification,
                        GroupTypeCode = organisationGroupTypeCode,
                        GroupTypeIdentifier = grouping.GroupTypeIdentifier.AsMatchingEnum<Enums.OrganisationGroupTypeIdentifier>(),
                        GroupReason = grouping.GroupingReason.AsMatchingEnum<Enums.OrganisationGroupingReason>(),
                        IdentifierValue = targetOrganisationGroup.Identifier,
                        Name = targetOrganisationGroup.Name,
                        Identifiers = targetOrganisationGroup.Identifiers,
                        SearchableName = Sanitiser.SanitiseName(targetOrganisationGroup.Name),
                        Providers = providerGrouping,
                    };

                    results.Add(organisationGroupResult);
                }
            }

            return results;
        }

        private Func<Provider, string> GetProviderFieldForGrouping(OrganisationGroupTypeIdentifier identifierType, OrganisationGroupTypeCode organisationGroupTypeCode, GroupingReason groupingReason)
        {
            // If the grouping reason is for payment, then the organisation identifier needs to be returned as a UKPRN, but grouped on the type code
            if (groupingReason == GroupingReason.Payment)
            {
                switch (organisationGroupTypeCode)
                {
                    case OrganisationGroupTypeCode.AcademyTrust:
                        return c => c.TrustCode;
                    case OrganisationGroupTypeCode.LocalAuthority:
                        return c => c.LACode;
                    case OrganisationGroupTypeCode.Provider:
                        return c => c.UKPRN;
                }
            }
            else
            {
                // TODO: Map all fields required
                switch (identifierType)
                {
                    case OrganisationGroupTypeIdentifier.UKPRN:
                        return c => c.UKPRN;
                    case OrganisationGroupTypeIdentifier.AcademyTrustCode:
                        return c => c.TrustCode;
                    case OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode:
                        return c => c.ParliamentaryConstituencyCode;
                    case OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode:
                        return c => c.MiddleSuperOutputAreaCode;
                    case OrganisationGroupTypeIdentifier.CensusWardCode:
                        return c => c.CensusWardCode;
                    case OrganisationGroupTypeIdentifier.DistrictCode:
                        return c => c.DistrictCode;
                    case OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode:
                        return c => c.GovernmentOfficeRegionCode;
                    case OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode:
                        return c => c.LowerSuperOutputAreaCode;
                    case OrganisationGroupTypeIdentifier.WardCode:
                        return c => c.WardCode;
                    case OrganisationGroupTypeIdentifier.RscRegionCode:
                        return c => c.RscRegionCode;
                    case OrganisationGroupTypeIdentifier.CountryCode:
                        return c => c.CountryCode;
                    case OrganisationGroupTypeIdentifier.LACode:
                        return c => c.LACode;
                    case OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode:
                        return c => c.LocalGovernmentGroupTypeCode;
                }
            }

            throw new Exception("Unknown OrganisationGroupTypeIdentifier for provider ID");
        }

        private bool ShouldIncludeProvider(Provider provider, IEnumerable<ProviderTypeMatch> providerTypeMatches)
        {
            foreach (ProviderTypeMatch providerTypeMatch in providerTypeMatches)
            {
                if (string.Equals(provider.ProviderType, providerTypeMatch.ProviderType, StringComparison.InvariantCultureIgnoreCase) && string.Equals(provider.ProviderSubType, providerTypeMatch.ProviderSubtype, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
