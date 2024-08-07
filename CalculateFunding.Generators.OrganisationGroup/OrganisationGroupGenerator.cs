﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.FundingDataZone;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Helpers;
using CalculateFunding.Common.Utility;
using CalculateFunding.Generators.OrganisationGroup.Extensions;
using CalculateFunding.Generators.OrganisationGroup.Interfaces;
using CalculateFunding.Generators.OrganisationGroup.Models;
using FdzPaymentOrganisation = CalculateFunding.Common.ApiClient.FundingDataZone.Models.PaymentOrganisation;

namespace CalculateFunding.Generators.OrganisationGroup
{
    public class OrganisationGroupGenerator : IOrganisationGroupGenerator
    {
        private readonly IOrganisationGroupTargetProviderLookup _organisationGroupTargetProviderLookup;
        private readonly IFundingDataZoneApiClient _fundingDataZoneApiClient;
        private readonly IProviderFilter _providerFilter;

        public OrganisationGroupGenerator(
            IOrganisationGroupTargetProviderLookup organisationGroupTargetProviderLookup, 
            IFundingDataZoneApiClient fundingDataZoneApiClient,
            IProviderFilter providerFilter)
        {
            _organisationGroupTargetProviderLookup = organisationGroupTargetProviderLookup;
            _fundingDataZoneApiClient = fundingDataZoneApiClient;
            _providerFilter = providerFilter;
        }

        public async Task<IEnumerable<OrganisationGroupResult>> GenerateOrganisationGroup(
            FundingConfiguration fundingConfiguration,
            IEnumerable<Provider> scopedProviders,
            string providerVersionId,
            int? providerSnapshotId = null)
        {
            if (!fundingConfiguration.OrganisationGroupings.Any())
            {
                List<OrganisationGroupingConfiguration> organisationGroupingConfigurations = new List<OrganisationGroupingConfiguration>();
                fundingConfiguration.ReleaseChannels.ToList().ForEach(_ =>
                {
                    _.OrganisationGroupings.ToList().ForEach(orgGroup =>
                    {
                        List<OrganisationGroupingConfiguration> duplicateGroupConfig = organisationGroupingConfigurations.Where(c => c.GroupingReason == orgGroup.GroupingReason
                                && c.OrganisationGroupTypeCode == orgGroup.OrganisationGroupTypeCode
                                && c.GroupTypeClassification == orgGroup.GroupTypeClassification
                                && c.GroupTypeIdentifier == orgGroup.GroupTypeIdentifier).ToList();
                        //Bug 145880 : Duplicate organization config can be seen in different channels. Avoiding duplicate and take the one which has providerStatus and ProviderTypeMatch
                        if (duplicateGroupConfig.Any())
                        {
                            duplicateGroupConfig.FirstOrDefault().ProviderStatus = (duplicateGroupConfig.FirstOrDefault().ProviderStatus == null || !duplicateGroupConfig.FirstOrDefault().ProviderStatus.Any())
                                        ? orgGroup.ProviderStatus
                                        : duplicateGroupConfig.FirstOrDefault().ProviderStatus;
                            duplicateGroupConfig.FirstOrDefault().ProviderTypeMatch = (duplicateGroupConfig.FirstOrDefault().ProviderStatus == null || !duplicateGroupConfig.FirstOrDefault().ProviderTypeMatch.Any())
                                        ? orgGroup.ProviderTypeMatch
                                        : duplicateGroupConfig.FirstOrDefault().ProviderTypeMatch;
                        }
                        else
                        {
                            organisationGroupingConfigurations.Add(orgGroup);
                        }
                    });
                });
                fundingConfiguration.OrganisationGroupings = organisationGroupingConfigurations;
            }

            return await GenerateOrganisationGroup(fundingConfiguration.OrganisationGroupings, fundingConfiguration.ProviderSource,
                fundingConfiguration.PaymentOrganisationSource, scopedProviders, providerVersionId, providerSnapshotId);
        }

        public async Task<IEnumerable<OrganisationGroupingConfiguration>> ExtractOrganizatioGroupConfig(FundingConfiguration fundingConfiguration)
        {
            if (fundingConfiguration.OrganisationGroupings == null)
            {
                List<OrganisationGroupingConfiguration> organisationGroupingConfigurations = new List<OrganisationGroupingConfiguration>();
                fundingConfiguration.ReleaseChannels.ToList().ForEach(_ =>
                {
                    _.OrganisationGroupings.ToList().ForEach(orgGroup =>
                    {
                        OrganisationGroupingConfiguration organisationGroupingConfiguration = new OrganisationGroupingConfiguration()
                        {
                            GroupingReason = orgGroup.GroupingReason,
                            GroupTypeClassification = orgGroup.GroupTypeClassification,
                            GroupTypeIdentifier = orgGroup.GroupTypeIdentifier,
                            OrganisationGroupTypeCode = orgGroup.OrganisationGroupTypeCode,
                            ProviderStatus = orgGroup.ProviderStatus,
                            ProviderTypeMatch = orgGroup.ProviderTypeMatch
                        };
                        organisationGroupingConfigurations.Add(organisationGroupingConfiguration);
                    });
                });
                fundingConfiguration.OrganisationGroupings = organisationGroupingConfigurations;
            }
            return fundingConfiguration.OrganisationGroupings;
        }

        public async Task<IEnumerable<string>> GetGroupTypeIdentifierList(FundingConfiguration fundingConfiguration)
        {
            List<string> groupTypeIdentifierList = new List<string>();
            IEnumerable<OrganisationGroupingConfiguration> extractedOrganizationGroupConfig = await ExtractOrganizatioGroupConfig(fundingConfiguration);
            return extractedOrganizationGroupConfig.Select(x => x.GroupTypeIdentifier.ToString()).Distinct().ToList();
        }

        public async Task<IEnumerable<OrganisationGroupResult>> GenerateOrganisationGroup(
            IEnumerable<OrganisationGroupingConfiguration> organisationGroupingConfigurations,
            ProviderSource providerSource,
            PaymentOrganisationSource paymentOrganisationSource,
            IEnumerable<Provider> scopedProviders,
            string providerVersionId,
            int? providerSnapshotId = null)
        {
            Guard.ArgumentNotNull(organisationGroupingConfigurations, nameof(organisationGroupingConfigurations));
            Guard.ArgumentNotNull(providerSource, nameof(providerSource));
            Guard.ArgumentNotNull(paymentOrganisationSource, nameof(paymentOrganisationSource));
            Guard.ArgumentNotNull(scopedProviders, nameof(scopedProviders));
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));

            List<OrganisationGroupResult> results = new List<OrganisationGroupResult>();

            Dictionary<string, FdzPaymentOrganisation> paymentOrganisations = new Dictionary<string, FdzPaymentOrganisation>();

            if (providerSource == ProviderSource.FDZ
                && organisationGroupingConfigurations.Any(g => g.GroupingReason == GroupingReason.Payment 
                                                                    || g.GroupingReason == GroupingReason.Contracting
                                                                    || g.GroupingReason == GroupingReason.Indicative))
            {
                if (!providerSnapshotId.HasValue)
                {
                    throw new InvalidOperationException("No provider snapshot ID provided, but it is required fto lookup Payment Organisations from FDZ");
                }

                ApiResponse<IEnumerable<FdzPaymentOrganisation>> paymentOrganisationsResponse = await _fundingDataZoneApiClient.GetAllOrganisations(providerSnapshotId.Value);
                if (paymentOrganisationsResponse.StatusCode == System.Net.HttpStatusCode.OK && paymentOrganisationsResponse.Content != null)
                {
                    foreach(FdzPaymentOrganisation fdzPaymentOrganisation in paymentOrganisationsResponse.Content)
                    {
                        if (paymentOrganisations.ContainsKey(fdzPaymentOrganisation.Ukprn))
                        {
                            throw new Exception($"The payment organisation group: '{fdzPaymentOrganisation.Ukprn}' needs to be unique for provider snapshot ID '{providerSnapshotId}'.");
                        }
                        else
                        {
                            paymentOrganisations.Add(fdzPaymentOrganisation.Ukprn, fdzPaymentOrganisation);
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Unable to retreive payment organisations from provider snapshot ID of {providerSnapshotId}");
                }
            }

            foreach (OrganisationGroupingConfiguration grouping in organisationGroupingConfigurations)
            {
                // Get the provider attribute required to group
                Func<Provider, string> providerFilterAttribute = GetProviderFieldForGrouping(
                    grouping.GroupTypeIdentifier, grouping.OrganisationGroupTypeCode, grouping.GroupingReason, paymentOrganisationSource);

                // Filter providers based on provider type and subtypes
                IEnumerable<Provider> providersForGroup = grouping.ProviderTypeMatch.IsNullOrEmpty() ? scopedProviders : scopedProviders.Where(_ => _providerFilter.ShouldIncludeProvider(_, grouping.ProviderTypeMatch));

                // Filter providers based on provider status
                providersForGroup = grouping.ProviderStatus.IsNullOrEmpty() ? providersForGroup : providersForGroup.Where(_ => _providerFilter.ShouldIncludeProvider(_, grouping.ProviderStatus));

                // Group providers by the fields and discard any providers with null values for that field
                IEnumerable<IGrouping<string, Provider>> groupedProviders = providersForGroup.GroupBy(providerFilterAttribute);

                // Common values for all groups
                Enums.OrganisationGroupTypeClassification organisationGroupTypeClassification = grouping.GroupingReason.IsForProviderPayment() ? Enums.OrganisationGroupTypeClassification.LegalEntity : Enums.OrganisationGroupTypeClassification.GeographicalBoundary;
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

                    if (paymentOrganisationSource == PaymentOrganisationSource.PaymentOrganisationFields
                        && grouping.GroupingReason.IsForProviderPayment())
                    {
                        IEnumerable<OrganisationIdentifier> identifiers;

                        // lookup alternative identifier and name from FDZ's PaymentOrganisation table via FDZ service
                        if (providerSource == ProviderSource.FDZ
                            && paymentOrganisations.TryGetValue(providerGrouping.Key, out FdzPaymentOrganisation fdzPaymentOrganisation))
                        {
                            identifiers = GetIdentifiers(fdzPaymentOrganisation);
                        }
                        else
                        {
                            identifiers = Array.Empty<OrganisationIdentifier>();
                        }

                        // Will use providerGrouping.Key as the identifier of the PaymentOrganisation
                        targetOrganisationGroup = new TargetOrganisationGroup()
                        {
                            Identifier = grouping.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider ? providerGrouping.First().ProviderId : providerGrouping.First().PaymentOrganisationIdentifier,
                            Name = grouping.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider ? providerGrouping.First().Name : providerGrouping.First().PaymentOrganisationName,
                            Identifiers = identifiers,
                        };
                    }
                    else if (paymentOrganisationSource == PaymentOrganisationSource.PaymentOrganisationAsProvider
                       || (paymentOrganisationSource == PaymentOrganisationSource.PaymentOrganisationFields
                                && !grouping.GroupingReason.IsForProviderPayment())
                       )
                    {
                        targetOrganisationGroup = await ObtainTargetOrganisationGroupFromProviderData(providerVersionId, grouping, providerGrouping);
                    }


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

        private IEnumerable<OrganisationIdentifier> GetIdentifiers(FdzPaymentOrganisation fdzPaymentOrganisation)
        {
            List<OrganisationIdentifier> identifiers = new List<OrganisationIdentifier>();
            if (!string.IsNullOrWhiteSpace(fdzPaymentOrganisation.Ukprn))
            {
                identifiers.Add(new OrganisationIdentifier() { Type = Enums.OrganisationGroupTypeIdentifier.UKPRN, Value = fdzPaymentOrganisation.Ukprn });
            }
            if (!string.IsNullOrWhiteSpace(fdzPaymentOrganisation.TrustCode))
            {
                identifiers.Add(new OrganisationIdentifier() { Type = Enums.OrganisationGroupTypeIdentifier.AcademyTrustCode, Value = fdzPaymentOrganisation.TrustCode });
            }
            if (!string.IsNullOrWhiteSpace(fdzPaymentOrganisation.Urn))
            {
                identifiers.Add(new OrganisationIdentifier() { Type = Enums.OrganisationGroupTypeIdentifier.URN, Value = fdzPaymentOrganisation.Urn });
            }
            if (!string.IsNullOrWhiteSpace(fdzPaymentOrganisation.LaCode))
            {
                identifiers.Add(new OrganisationIdentifier() { Type = Enums.OrganisationGroupTypeIdentifier.LACode, Value = fdzPaymentOrganisation.LaCode });
            }
            if (!string.IsNullOrWhiteSpace(fdzPaymentOrganisation.CompanyHouseNumber))
            {
                identifiers.Add(new OrganisationIdentifier() { Type = Enums.OrganisationGroupTypeIdentifier.CompaniesHouseNumber, Value = fdzPaymentOrganisation.CompanyHouseNumber });
            }

            return identifiers;
        }

        private async Task<TargetOrganisationGroup> ObtainTargetOrganisationGroupFromProviderData(string providerVersionId, OrganisationGroupingConfiguration grouping, IGrouping<string, Provider> providerGrouping)
        {
            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = providerGrouping.Key,
                OrganisationGroupTypeCode = grouping.OrganisationGroupTypeCode,
                ProviderVersionId = providerVersionId,
                GroupTypeIdentifier = grouping.GroupTypeIdentifier
            };

            TargetOrganisationGroup targetOrganisationGroup = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters, grouping.GroupingReason, providerGrouping);

            return targetOrganisationGroup;
        }

        private Func<Provider, string> GetProviderFieldForGrouping(OrganisationGroupTypeIdentifier identifierType, OrganisationGroupTypeCode organisationGroupTypeCode, GroupingReason groupingReason, PaymentOrganisationSource paymentOrganisationSource)
        {
            if (groupingReason.IsForProviderPayment())
            {
                if (organisationGroupTypeCode == OrganisationGroupTypeCode.Provider)
                {
                    return c => c.ProviderId;
                }

                if (paymentOrganisationSource == PaymentOrganisationSource.PaymentOrganisationAsProvider)
                {
                    // If the grouping reason is for payment, then the organisation identifier needs to be returned as a UKPRN, but grouped on the type code
                    switch (organisationGroupTypeCode)
                    {
                        case OrganisationGroupTypeCode.AcademyTrust:
                            return c => c.TrustCode;
                        case OrganisationGroupTypeCode.LocalAuthority:
                            return c => c.LACode;
                        case OrganisationGroupTypeCode.Provider:
                            return c => c.UKPRN;
                        default:
                            throw new InvalidOperationException($"Unknown organisation group type code to select for identifer for payment. '{organisationGroupTypeCode}'");
                    }
                }
                else if (paymentOrganisationSource == PaymentOrganisationSource.PaymentOrganisationFields)
                {
                    return p => p.PaymentOrganisationIdentifier;
                }
                else
                {
                    throw new InvalidOperationException($"Unknown paymentOrganisationSource '{paymentOrganisationSource}'");
                }
            }
            else if (groupingReason == GroupingReason.Information)
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
                    case OrganisationGroupTypeIdentifier.LondonRegionCode:
                        return c => c.LondonRegionCode;
                    case OrganisationGroupTypeIdentifier.LACode:
                        return c => c.LACode;
                    case OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode:
                        return c => c.LocalGovernmentGroupTypeCode;
                }
            }

            throw new Exception("Unknown OrganisationGroupTypeIdentifier for provider ID");
        }
    }
}
