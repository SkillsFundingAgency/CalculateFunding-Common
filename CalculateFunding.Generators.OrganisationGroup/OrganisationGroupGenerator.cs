using System;
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

        public OrganisationGroupGenerator(IOrganisationGroupTargetProviderLookup organisationGroupTargetProviderLookup, IFundingDataZoneApiClient fundingDataZoneApiClient)
        {
            _organisationGroupTargetProviderLookup = organisationGroupTargetProviderLookup;
            _fundingDataZoneApiClient = fundingDataZoneApiClient;
        }

        public async Task<IEnumerable<OrganisationGroupResult>> GenerateOrganisationGroup(
            FundingConfiguration fundingConfiguration,
            IEnumerable<Provider> scopedProviders,
            string providerVersionId,
            int? providerSnapshotId = null)
        {
            Guard.ArgumentNotNull(fundingConfiguration, nameof(fundingConfiguration));
            Guard.ArgumentNotNull(scopedProviders, nameof(scopedProviders));
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));

            List<OrganisationGroupResult> results = new List<OrganisationGroupResult>();

            Dictionary<string, FdzPaymentOrganisation> paymentOrganisations = new Dictionary<string, FdzPaymentOrganisation>();

            if (fundingConfiguration.ProviderSource == ProviderSource.FDZ
                && fundingConfiguration.OrganisationGroupings.Any(g => g.GroupingReason == GroupingReason.Payment || g.GroupingReason == GroupingReason.Contracting))
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

            foreach (OrganisationGroupingConfiguration grouping in fundingConfiguration.OrganisationGroupings)
            {
                // Get the provider attribute required to group
                Func<Provider, string> providerFilterAttribute = GetProviderFieldForGrouping(grouping.GroupTypeIdentifier, grouping.OrganisationGroupTypeCode, grouping.GroupingReason, fundingConfiguration.PaymentOrganisationSource);

                // Filter providers based on provider type and subtypes
                IEnumerable<Provider> providersForGroup = grouping.ProviderTypeMatch.IsNullOrEmpty() ? scopedProviders : scopedProviders.Where(_ => ShouldIncludeProvider(_, grouping.ProviderTypeMatch));

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

                    if (fundingConfiguration.PaymentOrganisationSource == PaymentOrganisationSource.PaymentOrganisationFields
                        && grouping.GroupingReason.IsForProviderPayment())
                    {
                        IEnumerable<OrganisationIdentifier> identifiers;

                        // lookup alternative identifier and name from FDZ's PaymentOrganisation table via FDZ service
                        if (fundingConfiguration.ProviderSource == ProviderSource.FDZ
                            && paymentOrganisations.TryGetValue(providerGrouping.Key, out FdzPaymentOrganisation fdzPaymentOrganisation))
                        {
                            identifiers = GetIdentifiers(fdzPaymentOrganisation);
                        }
                        else
                        {
                            identifiers = new OrganisationIdentifier[0];
                        }

                        // Will use providerGrouping.Key as the identifier of the PaymentOrganisation
                        targetOrganisationGroup = new TargetOrganisationGroup()
                        {
                            Identifier = providerGrouping.First().PaymentOrganisationIdentifier,
                            Name = providerGrouping.First().PaymentOrganisationName,
                            Identifiers = identifiers,
                        };
                    }
                    else if (fundingConfiguration.PaymentOrganisationSource == PaymentOrganisationSource.PaymentOrganisationAsProvider
                       || (fundingConfiguration.PaymentOrganisationSource == PaymentOrganisationSource.PaymentOrganisationFields
                                && !grouping.GroupingReason.IsForProviderPayment())
                       )
                    {
                        targetOrganisationGroup = await ObtainTargetOrganisationGroupFromProviderData(fundingConfiguration, providerVersionId, grouping, providerGrouping, targetOrganisationGroup);
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
        private async Task<TargetOrganisationGroup> ObtainTargetOrganisationGroupFromProviderData(FundingConfiguration fundingConfiguration, string providerVersionId, OrganisationGroupingConfiguration grouping, IGrouping<string, Common.ApiClient.Providers.Models.Provider> providerGrouping, TargetOrganisationGroup targetOrganisationGroup)
        {
            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = providerGrouping.Key,
                OrganisationGroupTypeCode = grouping.OrganisationGroupTypeCode,
                ProviderVersionId = providerVersionId,
                GroupTypeIdentifier = grouping.GroupTypeIdentifier
            };

            targetOrganisationGroup = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters, grouping.GroupingReason, providerGrouping);

            return targetOrganisationGroup;
        }

        private Func<Provider, string> GetProviderFieldForGrouping(OrganisationGroupTypeIdentifier identifierType, OrganisationGroupTypeCode organisationGroupTypeCode, GroupingReason groupingReason, PaymentOrganisationSource paymentOrganisationSource)
        {
            if (groupingReason.IsForProviderPayment())
            {
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
