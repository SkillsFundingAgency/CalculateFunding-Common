using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Providers;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Utility;
using CalculateFunding.Generators.OrganisationGroup.Interfaces;
using CalculateFunding.Generators.OrganisationGroup.Models;

namespace CalculateFunding.Generators.OrganisationGroup
{
    public class OrganisationGroupTargetProviderLookup : IOrganisationGroupTargetProviderLookup
    {
        private readonly IProvidersApiClient _providersApiClient;

        // TODO: pass in Policy for providers client
        public OrganisationGroupTargetProviderLookup(IProvidersApiClient providersApiClient)
        {
            Guard.ArgumentNotNull(providersApiClient, nameof(providersApiClient));

            _providersApiClient = providersApiClient;
        }

        /// <summary>
        /// Returns the target organisations groups details, based on the configuration of the funding output.
        /// eg will lookup
        /// - UKPRN and name for a Local Authority to pay
        /// - UKRPN and name of a multi academy trust to pay
        /// - Parlimentary Consituency code and name, based on the Parlimentary Consituency code for information
        /// </summary>
        /// <param name="identifierValue">Identifier value of the Organisation Group</param>
        /// <param name="groupingReason">Grouping Reason</param>
        /// <param name="organisationGroupTypeCode">Organisation Group Type Code</param>
        /// <param name="groupTypeIdentifier">Group Type Identifier</param>
        /// <returns></returns>
        public async Task<TargetOrganisationGroup> GetTargetProviderDetails(string identifierValue, GroupingReason groupingReason, OrganisationGroupTypeCode organisationGroupTypeCode, OrganisationGroupTypeIdentifier groupTypeIdentifier, string providerVersionId, IEnumerable<Provider> providersInGroup)
        {
            IEnumerable<Provider> allProviders = await GetAllProviders(providerVersionId);
            Provider targetProvider = null;
            if (groupingReason == GroupingReason.Payment)
            {
                // Always return a UKRPN, as we need to pay a LegalEntity

                if (organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust)
                {
                    // Single academy trust
                    if (providersInGroup.Count() == 1 && providersInGroup.First().TrustStatus == TrustStatus.SupportedByASingleAacademyTrust)
                    {
                        targetProvider = allProviders.SingleOrDefault(p => p.TrustCode == identifierValue);
                    }
                    else
                    {
                        // Lookup by multi academy trust. NOTE: actual data does not contain the multi academy trust entity
                        targetProvider = allProviders.SingleOrDefault(p => p.TrustCode == identifierValue && p.ProviderType == "Multi-academy trust");
                    }

                }
                else if (organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority)
                {
                    // Lookup the local authority by LACode and provider type and subtype
                    targetProvider = allProviders.SingleOrDefault(p => p.ProviderType == "Local Authority" && p.ProviderSubType == "Local Authority" && p.LACode == identifierValue);
                }
                else if (organisationGroupTypeCode == OrganisationGroupTypeCode.Provider)
                {
                    targetProvider = allProviders.SingleOrDefault(p => p.UKPRN == identifierValue);
                }
                else
                {
                    throw new Exception("Unable to lookup target provider, given the OrganisationGroupTypeCode");
                }

                // Return the provider if found. TODO add null checks and exceptions
                return new TargetOrganisationGroup()
                {
                    Identifier = targetProvider.UKPRN,
                    Name = targetProvider.Name,
                    Identifiers = Enum.GetValues(typeof(OrganisationGroupTypeIdentifier)).Cast<OrganisationGroupTypeIdentifier>().SelectMany(x => GenerateIdentifiersForProvider(targetProvider, x)),
                };
            }
            else
            {
                // Get the first of the scoped providers, then obtain the ID and name from the properties of that.
                Provider firstProvider = providersInGroup.First();

                return new TargetOrganisationGroup()
                {
                    Name = GetOrganisationGroupName(firstProvider, groupTypeIdentifier),
                    Identifier = GetOrganisationGroupIdentifier(firstProvider, groupTypeIdentifier),
                    Identifiers = Enum.GetValues(typeof(OrganisationGroupTypeIdentifier)).Cast<OrganisationGroupTypeIdentifier>().SelectMany(x => GenerateIdentifiersForProvider(firstProvider, x)),
                };
            }
        }

        private IEnumerable<OrganisationIdentifier> GenerateIdentifiersForProvider(Provider targetProvider, OrganisationGroupTypeIdentifier organisationGroupTypeIdentifier)
        {
            string targetProviderGroupTypeIdentifierValue = GetOrganisationGroupIdentifier(targetProvider, organisationGroupTypeIdentifier);
            if (!string.IsNullOrWhiteSpace(targetProviderGroupTypeIdentifierValue))
            {
                yield return new OrganisationIdentifier { Type = organisationGroupTypeIdentifier.AsMatchingEnum<Enums.OrganisationGroupTypeIdentifier>(), Value = targetProviderGroupTypeIdentifierValue };
            }
        }

        private async Task<IEnumerable<Provider>> GetAllProviders(string providerVersionId)
        {
            // Cache this call within the instance of this class

            return (await _providersApiClient.GetProvidersByVersion(providerVersionId)).Content.Providers;

        }

        private string GetOrganisationGroupIdentifier(Provider provider, OrganisationGroupTypeIdentifier identifierType)
        {
            // TODO - finish list of mappings
            switch (identifierType)
            {
                case OrganisationGroupTypeIdentifier.AcademyTrustCode:
                    return provider.TrustCode;
                case OrganisationGroupTypeIdentifier.UID:
                case OrganisationGroupTypeIdentifier.GroupId:
                    return string.Empty;
                default:
                    return identifierType.PropertyMapping(provider)?.ToString();
            }
        }

        private string GetOrganisationGroupName(Provider provider, OrganisationGroupTypeIdentifier identifierType)
        {
            // TODO - finish list of mappings
            switch (identifierType)
            {
                case OrganisationGroupTypeIdentifier.AcademyTrustCode:
                    return provider.TrustName;
                case OrganisationGroupTypeIdentifier.UKPRN:
                    return provider.Name;
                case OrganisationGroupTypeIdentifier.LACode:
                    return provider.Authority;
                case OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode:
                    return provider.ParliamentaryConstituencyName;
                case OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode:
                    return provider.MiddleSuperOutputAreaName;
                case OrganisationGroupTypeIdentifier.CensusWardCode:
                    return provider.CensusWardName;
                case OrganisationGroupTypeIdentifier.DistrictCode:
                    return provider.DistrictName;
                case OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode:
                    return provider.GovernmentOfficeRegionName;
                case OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode:
                    return provider.LowerSuperOutputAreaName;
                case OrganisationGroupTypeIdentifier.WardCode:
                    return provider.WardName;
                case OrganisationGroupTypeIdentifier.RscRegionCode:
                    return provider.RscRegionName;
                case OrganisationGroupTypeIdentifier.CountryCode:
                    return provider.CountryName;
                default:
                    throw new Exception("Unable to resolve field to identifier value");
            }
        }
    }
}
