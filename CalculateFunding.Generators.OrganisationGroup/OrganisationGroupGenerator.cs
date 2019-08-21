using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Generators.OrganisationGroup.Interfaces;
using CalculateFunding.Generators.OrganisationGroup.Models;

namespace CalculateFunding.Generators.OrganisationGroup
{
    public class OrganisationGroupGenerator
    {
        private readonly IOrganisationGroupTargetProviderLookup _organisationGroupTargetProviderLookup;

        public OrganisationGroupGenerator(IOrganisationGroupTargetProviderLookup organisationGroupTargetProviderLookup)
        {
            _organisationGroupTargetProviderLookup = organisationGroupTargetProviderLookup;
        }

        public async Task<IEnumerable<OrganisationGroupResult>> GenerateOrganisationGroup(FundingConfiguration fundingConfiguration, IEnumerable<Provider> scopedProviders, string providerVersionId)
        {
            List<OrganisationGroupResult> results = new List<OrganisationGroupResult>();

            foreach (OrganisationGroupingConfiguration grouping in fundingConfiguration.OrganisationGroupings)
            {
                // Get the provider attribute required to group
                Func<Provider, string> providerFilterAttribute = GetProviderFieldForGrouping(grouping.GroupTypeIdentifier, grouping.OrganisationGroupTypeCode, grouping.GroupingReason);

                // Filter providers based on provider type and subtypes
                IEnumerable<Provider> providersForGroup = FilterProviders(scopedProviders, grouping.ProviderTypeMatch);

                // Group providers by the fields and discard any providers with null values for that field
                IEnumerable<IGrouping<string, Provider>> groupedProviders = providersForGroup.GroupBy(providerFilterAttribute);

                // Common values for all groups
                Enums.OrganisationGroupTypeClassification organisationGroupTypeClassification = grouping.GroupingReason == GroupingReason.Payment ? Enums.OrganisationGroupTypeClassification.LegalEntity : Enums.OrganisationGroupTypeClassification.GeographicalBoundary;
                Enums.OrganisationGroupTypeCode organisationGroupTypeCode = GetGroupTypeCode(grouping.OrganisationGroupTypeCode);

                // Generate Organisation Group results based on the grouped providers
                foreach (IGrouping<string, Provider> providerGrouping in groupedProviders)
                {
                    // Ignore providers without the matching data in the key
                    if (string.IsNullOrWhiteSpace(providerGrouping.Key))
                    {
                        continue;
                    }

                    TargetOrganisationGroup targetOrganisationGroup = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(providerGrouping.Key, grouping.GroupingReason, grouping.OrganisationGroupTypeCode, grouping.GroupTypeIdentifier, providerVersionId, providerGrouping);
                    if (targetOrganisationGroup == null)
                    {
                        // TODO: improve logging
                        throw new Exception($"Target Organisation Group could not be found for identifier '{providerGrouping.Key}'");
                    }

                    OrganisationGroupResult organisationGroupResult = new OrganisationGroupResult()
                    {
                        GroupTypeClassification = organisationGroupTypeClassification,
                        GroupTypeCode = organisationGroupTypeCode,
                        GroupTypeIdentifier = GetGroupTypeIdentifier(grouping.GroupTypeIdentifier),
                        IdentifierValue = targetOrganisationGroup.Identifier,
                        Name = targetOrganisationGroup.Name,
                        Identifiers = targetOrganisationGroup.Identifiers,
                        SearchableName = GenerateSearchableName(targetOrganisationGroup.Name),
                        Providers = providerGrouping,
                    };

                    results.Add(organisationGroupResult);
                }
            }

            return results;
        }

        /// <summary>
        /// Move this to a common package somewhere and ask MyESF for the implementation
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Searchable name compatible with Azure Search</returns>
        private string GenerateSearchableName(string name)
        {
            return name.Replace(" ", "");
        }

        private Enums.OrganisationGroupTypeIdentifier GetGroupTypeIdentifier(OrganisationGroupTypeIdentifier organisationGroupingType)
        {
            switch (organisationGroupingType)
            {
                case OrganisationGroupTypeIdentifier.UKPRN:
                    return Enums.OrganisationGroupTypeIdentifier.UKPRN;
                case OrganisationGroupTypeIdentifier.LACode:
                    return Enums.OrganisationGroupTypeIdentifier.LACode;
                case OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode:
                    return Enums.OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode;
                default:
                    throw new Exception("OrganisationGroupTypeIdentifier not found");
            }
        }

        private Enums.OrganisationGroupTypeCode GetGroupTypeCode(OrganisationGroupTypeCode organisationGroupingType)
        {
            // TODO - map all types
            switch (organisationGroupingType)
            {
                case OrganisationGroupTypeCode.LocalAuthority:
                    return Enums.OrganisationGroupTypeCode.LocalAuthority;
                case OrganisationGroupTypeCode.Provider:
                    return Enums.OrganisationGroupTypeCode.Provider;
                case OrganisationGroupTypeCode.ParliamentaryConstituency:
                    return Enums.OrganisationGroupTypeCode.ParliamentaryConstituency;
                default:
                    throw new Exception("Unknown type code");
            }
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
                }
            }


            throw new Exception("Unknown type");
        }

        private IEnumerable<Provider> FilterProviders(IEnumerable<Provider> scopedProviders, IEnumerable<ProviderTypeMatch> providerTypeMatch)
        {
            // Todo - filter based on provider type and provider subtype against the provider matches. Include in list if it matches any provider type and subtype

            return scopedProviders;
        }
    }
}
