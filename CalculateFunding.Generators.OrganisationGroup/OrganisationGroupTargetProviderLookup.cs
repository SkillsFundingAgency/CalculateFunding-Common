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
using Polly;

namespace CalculateFunding.Generators.OrganisationGroup
{
    public class OrganisationGroupTargetProviderLookup : IOrganisationGroupTargetProviderLookup
    {
        private readonly IProvidersApiClient _providersApiClient;
        private readonly AsyncPolicy _providersApiClientPolicy;
        private readonly IDictionary<GroupingReason, Dictionary<OrganisationGroupTypeCode, IEnumerable<OrganisationGroupTypeIdentifier>>> _additionalIdentifierKeys;

        private Dictionary<string, Dictionary<string, Provider>> _providers = null;

        private readonly IEnumerable<string> _academyTrustTypes = new string[] { "Academy trust" };

        public OrganisationGroupTargetProviderLookup(IProvidersApiClient providersApiClient, IOrganisationGroupResiliencePolicies resiliencePolicies)
        {
            Guard.ArgumentNotNull(providersApiClient, nameof(providersApiClient));
            Guard.ArgumentNotNull(resiliencePolicies, nameof(resiliencePolicies));

            _providersApiClient = providersApiClient;
            _providersApiClientPolicy = resiliencePolicies.ProvidersApiClient;
            _additionalIdentifierKeys = GenerateAdditionalKeys();
            _providers = new Dictionary<string, Dictionary<string, Provider>>();
        }

        private Dictionary<GroupingReason, Dictionary<OrganisationGroupTypeCode, IEnumerable<OrganisationGroupTypeIdentifier>>> GenerateAdditionalKeys()
        {
            Dictionary<GroupingReason, Dictionary<OrganisationGroupTypeCode, IEnumerable<OrganisationGroupTypeIdentifier>>> additionalIdentifierKeys = new Dictionary<GroupingReason, Dictionary<OrganisationGroupTypeCode, IEnumerable<OrganisationGroupTypeIdentifier>>>();

            Dictionary<OrganisationGroupTypeCode, IEnumerable<OrganisationGroupTypeIdentifier>> paymentKeys = new Dictionary<OrganisationGroupTypeCode, IEnumerable<OrganisationGroupTypeIdentifier>>();

            paymentKeys.Add(OrganisationGroupTypeCode.AcademyTrust, new OrganisationGroupTypeIdentifier[] { OrganisationGroupTypeIdentifier.UKPRN, OrganisationGroupTypeIdentifier.URN, OrganisationGroupTypeIdentifier.CompaniesHouseNumber, OrganisationGroupTypeIdentifier.GroupId, OrganisationGroupTypeIdentifier.AcademyTrustCode });
            paymentKeys.Add(OrganisationGroupTypeCode.LocalAuthority, new OrganisationGroupTypeIdentifier[] { OrganisationGroupTypeIdentifier.UKPRN, OrganisationGroupTypeIdentifier.LACode, OrganisationGroupTypeIdentifier.UPIN, OrganisationGroupTypeIdentifier.URN, OrganisationGroupTypeIdentifier.UID, OrganisationGroupTypeIdentifier.DfeEstablishmentNumber });

            additionalIdentifierKeys.Add(GroupingReason.Payment, paymentKeys);

            Dictionary<OrganisationGroupTypeCode, IEnumerable<OrganisationGroupTypeIdentifier>> informationKeys = new Dictionary<OrganisationGroupTypeCode, IEnumerable<OrganisationGroupTypeIdentifier>>();

            informationKeys.Add(OrganisationGroupTypeCode.AcademyTrust, new OrganisationGroupTypeIdentifier[] { OrganisationGroupTypeIdentifier.UKPRN, OrganisationGroupTypeIdentifier.URN, OrganisationGroupTypeIdentifier.CompaniesHouseNumber, OrganisationGroupTypeIdentifier.GroupId, OrganisationGroupTypeIdentifier.AcademyTrustCode });
            informationKeys.Add(OrganisationGroupTypeCode.LocalAuthority, new OrganisationGroupTypeIdentifier[] { OrganisationGroupTypeIdentifier.LACode, OrganisationGroupTypeIdentifier.LAOrg });
            informationKeys.Add(OrganisationGroupTypeCode.GovernmentOfficeRegion, new OrganisationGroupTypeIdentifier[] { OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode });
            informationKeys.Add(OrganisationGroupTypeCode.LocalGovernmentGroup, new OrganisationGroupTypeIdentifier[] { OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode });
            informationKeys.Add(OrganisationGroupTypeCode.Provider, new OrganisationGroupTypeIdentifier[] { OrganisationGroupTypeIdentifier.UKPRN, OrganisationGroupTypeIdentifier.LACode, OrganisationGroupTypeIdentifier.UPIN, OrganisationGroupTypeIdentifier.URN, OrganisationGroupTypeIdentifier.UID, OrganisationGroupTypeIdentifier.CompaniesHouseNumber });

            additionalIdentifierKeys.Add(GroupingReason.Information, informationKeys);

            return additionalIdentifierKeys;
        }

        /// <summary>
        /// Returns the target organisations groups details, based on the configuration of the funding output.
        /// eg will lookup
        /// - Parlimentary Consituency code and name, based on the Parlimentary Consituency code for information
        /// </summary>
        /// <param name="organisationGroupLookupParameters">Grouping Lookup Parameters</param>
        /// <param name="providersInGroup">Providers in group</param>
        /// <returns></returns>
        public async Task<TargetOrganisationGroup> GetTargetProviderDetails(OrganisationGroupLookupParameters organisationGroupLookupParameters,
            GroupingReason groupReason, IEnumerable<Provider> providersInGroup)
        {
            Guard.ArgumentNotNull(organisationGroupLookupParameters, nameof(organisationGroupLookupParameters));

            if (groupReason == GroupingReason.Payment || groupReason == GroupingReason.Contracting || groupReason == GroupingReason.Indicative)
            {
                Guard.IsNullOrWhiteSpace(organisationGroupLookupParameters.IdentifierValue, nameof(organisationGroupLookupParameters.IdentifierValue));
                Guard.IsNullOrWhiteSpace(organisationGroupLookupParameters.ProviderVersionId, nameof(organisationGroupLookupParameters.ProviderVersionId));

                return await GetTargetProviderDetailsForPaymentOrContracting(organisationGroupLookupParameters.IdentifierValue, organisationGroupLookupParameters.OrganisationGroupTypeCode, organisationGroupLookupParameters.ProviderVersionId, providersInGroup);
            }
            else
            {
                Guard.ArgumentNotNull(organisationGroupLookupParameters.GroupTypeIdentifier, nameof(organisationGroupLookupParameters.GroupTypeIdentifier));

                // Get the first of the scoped providers, then obtain the ID and name from the properties of that.
                Provider firstProvider = providersInGroup.First();

                return new TargetOrganisationGroup()
                {
                    Name = GetOrganisationGroupName(firstProvider, organisationGroupLookupParameters.GroupTypeIdentifier.Value),
                    Identifier = GetOrganisationGroupIdentifier(firstProvider, organisationGroupLookupParameters.GroupTypeIdentifier.Value),
                    Identifiers = organisationGroupLookupParameters.OrganisationGroupTypeCode.HasValue ? GenerateIdentifiersForProvider(groupReason, organisationGroupLookupParameters.OrganisationGroupTypeCode.Value, firstProvider) : null
                };
            }
        }

        /// <summary>
        /// Returns the target organisations groups details, based on the configuration of the funding output.
        /// eg will lookup
        /// - UKPRN and name for a Local Authority to pay
        /// - UKRPN and name of a multi academy trust to pay
        /// </summary>
        /// <param name="identifierValue">Identifier value of the Organisation Group</param>
        /// <param name="organisationGroupTypeCode">Organisation Group Type Code</param>
        /// <param name="providerVersionId">Provider version</param>
        /// <param name="providersInGroup">Providers in group</param>
        /// <returns></returns>
        private async Task<TargetOrganisationGroup> GetTargetProviderDetailsForPaymentOrContracting(string identifierValue, OrganisationGroupTypeCode? organisationGroupTypeCode, string providerVersionId, IEnumerable<Provider> providersInGroup)
        {
            if (!organisationGroupTypeCode.HasValue)
            {
                throw new Exception("Unable to lookup target provider, no OrganisationGroupTypeCode given");
            }

            Provider targetProvider = null;

            // Always return a UKRPN, as we need to pay a LegalEntity
            if (organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust || organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority)
            {
                Dictionary<string, Provider> allProviders = await GetAllProviders(providerVersionId);

                if (organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority)
                {
                    // Lookup the local authority by LACode and provider type and subtype
                    targetProvider = allProviders.Values.SingleOrDefault(p => p.ProviderType == "Local Authority" && p.ProviderSubType == "Local Authority" && p.LACode == identifierValue);
                }
                else if (organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust)
                {
                    // Lookup by multi academy trust. NOTE: actual data does not contain the multi academy trust entity
                    targetProvider = allProviders.Values.SingleOrDefault(p => p.TrustCode == identifierValue && _academyTrustTypes.Any(_ => p.ProviderType.Equals(_, StringComparison.OrdinalIgnoreCase) || p.ProviderSubType.Equals(_, StringComparison.OrdinalIgnoreCase)));

                    if (targetProvider == null && !providersInGroup.IsNullOrEmpty())
                    {
                        targetProvider = new Provider
                        {
                            ProviderId = identifierValue,
                            UKPRN = identifierValue,
                            TrustCode = identifierValue,
                            Name = providersInGroup?.First().TrustName,
                            PaymentOrganisationIdentifier = providersInGroup?.First().ProviderId,
                            PaymentOrganisationName = providersInGroup?.First().Name
                        };
                    }
                }
            }
            else if (organisationGroupTypeCode == OrganisationGroupTypeCode.Provider)
            {
                targetProvider = providersInGroup.SingleOrDefault(p => p.UKPRN == identifierValue);
            }
            else
            {
                throw new Exception($"Unable to lookup target provider, unsupported OrganisationGroupTypeCode of '{organisationGroupTypeCode}'");
            }

            if (targetProvider == null)
            {
                throw new Exception($"Unable to find target provider, given the OrganisationGroupTypeCode. Identifier = '{identifierValue}'. OrganisationGroupTypeCode= '{organisationGroupTypeCode}'");
            }

            // Return the provider if found.
            IEnumerable<OrganisationIdentifier> identifiers = GenerateIdentifiersForProvider(GroupingReason.Payment, organisationGroupTypeCode.Value, targetProvider);

            return new TargetOrganisationGroup()
            {
                Identifier = targetProvider.UKPRN,
                Name = targetProvider.Name,
                Identifiers = identifiers
            };
        }

        private IEnumerable<OrganisationIdentifier> GenerateIdentifiersForProvider(GroupingReason groupingReason, OrganisationGroupTypeCode organisationGroupTypeCode, Provider targetProvider)
        {
            if (_additionalIdentifierKeys[groupingReason].ContainsKey(organisationGroupTypeCode))
            {
                foreach (OrganisationGroupTypeIdentifier organisationGroupTypeIdentifier in _additionalIdentifierKeys[groupingReason][organisationGroupTypeCode])
                {
                    string targetProviderGroupTypeIdentifierValue = GetOrganisationGroupIdentifier(targetProvider, organisationGroupTypeIdentifier);
                    if (!string.IsNullOrWhiteSpace(targetProviderGroupTypeIdentifierValue))
                    {
                        Enums.OrganisationGroupTypeIdentifier typeIdentifier;

                        if (organisationGroupTypeIdentifier == OrganisationGroupTypeIdentifier.DfeEstablishmentNumber)
                        {
                            typeIdentifier = Enums.OrganisationGroupTypeIdentifier.DfeNumber;
                        }
                        else if (organisationGroupTypeIdentifier == OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode)
                        {
                            typeIdentifier = Enums.OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode;
                        }
                        else if (organisationGroupTypeIdentifier == OrganisationGroupTypeIdentifier.LAOrg)
                        {
                            typeIdentifier = Enums.OrganisationGroupTypeIdentifier.UKPRN;
                        }
                        else
                        {
                            typeIdentifier = organisationGroupTypeIdentifier.AsMatchingEnum<Enums.OrganisationGroupTypeIdentifier>();
                        }

                        yield return new OrganisationIdentifier { Type = typeIdentifier, Value = targetProviderGroupTypeIdentifierValue };
                    }
                }
            }
        }

        private async Task<Dictionary<string, Provider>> GetAllProviders(string providerVersionId)
        {
            if (!_providers.ContainsKey(providerVersionId))
            {
                Common.ApiClient.Models.ApiResponse<ProviderVersion> providerResult = await _providersApiClientPolicy.ExecuteAsync(() => _providersApiClient.GetProvidersByVersion(providerVersionId));
                if (providerResult == null)
                {
                    throw new InvalidOperationException($"Provider lookup returned null for provider version id '{providerVersionId}'");
                }

                if (providerResult.Content == null)
                {
                    throw new InvalidOperationException($"Provider lookup content returned null for provider version id '{providerVersionId}'");
                }

                if (providerResult.Content.Providers == null)
                {
                    throw new InvalidOperationException($"Provider lookup provider content returned null for provider version id '{providerVersionId}'");
                }


                _providers.Add(providerVersionId, providerResult.Content.Providers.ToDictionary(c => c.ProviderId, p => p));
            }

            return _providers[providerVersionId];
        }

        private string GetOrganisationGroupIdentifier(Provider provider, OrganisationGroupTypeIdentifier identifierType)
        {
            switch (identifierType)
            {
                case OrganisationGroupTypeIdentifier.AcademyTrustCode:
                    return provider.TrustCode;
                case OrganisationGroupTypeIdentifier.UID:
                case OrganisationGroupTypeIdentifier.GroupId:
                    return string.Empty;
                case OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode:
                    return provider.LocalGovernmentGroupTypeCode;
                default:
                    return identifierType.PropertyMapping(provider)?.ToString();
            }
        }

        private string GetOrganisationGroupName(Provider provider, OrganisationGroupTypeIdentifier identifierType)
        {
            switch (identifierType)
            {
                case OrganisationGroupTypeIdentifier.AcademyTrustCode:
                    return provider.TrustName;
                case OrganisationGroupTypeIdentifier.UKPRN:
                    return provider.Name;
                case OrganisationGroupTypeIdentifier.LACode:
                    return provider.Authority;
                case OrganisationGroupTypeIdentifier.DfeEstablishmentNumber:
                    return provider.DfeEstablishmentNumber;
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
                case OrganisationGroupTypeIdentifier.LondonRegionCode:
                    return provider.LondonRegionName;
                case OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode:
                    return provider.LocalGovernmentGroupTypeName;
                default:
                    throw new Exception("Unable to resolve field to identifier value");
            }
        }
    }
}
