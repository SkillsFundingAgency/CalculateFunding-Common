using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Providers;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Generators.OrganisationGroup.Interfaces;
using CalculateFunding.Generators.OrganisationGroup.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Polly;

namespace CalculateFunding.Generators.OrganisationGroup.UnitTests
{
    [TestClass]
    public class OrganisationGroupTargetProviderLookupTests
    {
        private IProvidersApiClient _providersApiClient;
        private IOrganisationGroupTargetProviderLookup _organisationGroupTargetProviderLookup;
        private string _providerVersionId;
        public Policy MockCacheProviderPolicy { get; set; } = Policy.NoOpAsync();

        [TestInitialize]
        public void Setup()
        {
            _providersApiClient = Substitute.For<IProvidersApiClient>();
            _organisationGroupTargetProviderLookup = new OrganisationGroupTargetProviderLookup(_providersApiClient, new OrganisationGroupResiliencePolicies
            {
                ProvidersApiClient = Policy.NoOpAsync()
            });
            _providerVersionId = "test-providers";
            _providersApiClient
                .GetProvidersByVersion(Arg.Is(_providerVersionId))
                .Returns(GetProviderVersion());
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnLocalAuthorityPayment_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = "101",
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.LocalAuthority,
                ProviderVersionId = _providerVersionId
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(
                organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Payment,
                new List<Provider> { scopedProviders.Where(_ => _.TrustCode == "101").First() });

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Provider 1");

            group.Identifier
                .Should()
                .Be("1001");

            group.Identifiers.Count()
                .Should()
                .Be(5);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.UKPRN)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.LACode)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.DfeNumber)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.CountryCode)
                .Should()
                .Be(true);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnProviderPayment_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = "1003",
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.Provider,
                ProviderVersionId = _providerVersionId
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Payment,
                new List<Provider> { scopedProviders.Where(_ => _.TrustCode == "102").First() }
                );

            group.Name
                .Should()
                .Be("Provider 3");

            group.Identifier
                .Should()
                .Be("1003");

            group.Identifiers
                .Should()
                .BeEmpty();
        }

        [TestMethod]
        public void WhenLookingUpTargetOrganisationGroupBasedOnNoGroupTypeCodePayment_ThenExceptionIsThrown()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = "1002",
                ProviderVersionId = _providerVersionId
            };

            Func<Task> action = async () => await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Payment,
                new List<Provider> { scopedProviders.Where(_ => _.TrustCode == "102").First() });

            action.Should().Throw<Exception>().WithMessage("Unable to lookup target provider, no OrganisationGroupTypeCode given");
        }

        [TestMethod]
        public void WhenLookingUpTargetOrganisationGroupBasedOnUnknownGroupTypePayment_ThenExceptionIsThrown()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = "1002",
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.District,
                ProviderVersionId = _providerVersionId
            };

            Func<Task> action = async () => await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Payment,
                new List<Provider> { scopedProviders.Where(_ => _.TrustCode == "102").First() });

            action.Should().Throw<Exception>().WithMessage("Unable to lookup target provider, unsupported OrganisationGroupTypeCode of 'District'");
        }

        [TestMethod]
        public void WhenLookingUpTargetOrganisationGroupBasedOnUnknownIdentifierPayment_ThenExceptionIsThrown()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = "1002",
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.AcademyTrust,
                ProviderVersionId = _providerVersionId
            };

            Func<Task> action = async () => await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Payment,
                new List<Provider> { });

            action.Should().Throw<Exception>().WithMessage("Unable to find target provider, given the OrganisationGroupTypeCode. Identifier = '1002'. OrganisationGroupTypeCode= 'AcademyTrust'");
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnAcadmeyTrustPayment_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = "102",
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.AcademyTrust,
                ProviderVersionId = _providerVersionId
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Payment,
                new List<Provider> { scopedProviders.Where(_ => _.TrustCode == "102").First() });

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Academy Trust 2");

            group.Identifier
                .Should()
                .Be("102");

            group.Identifiers.Count()
                .Should()
                .Be(2);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnMultiAcadmeyTrustPayment_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                IdentifierValue = "101",
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.AcademyTrust,
                ProviderVersionId = _providerVersionId
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Payment,
                new List<Provider> { scopedProviders.Where(_ => _.TrustCode == "101").First() });

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Multi provider");

            group.Identifier
                .Should()
                .Be("1001");

            group.Identifiers.Count()
                .Should()
                .Be(2);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnAcadmeyTrustInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.AcademyTrustCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.AcademyTrust
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders);

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Academy Trust 1");

            group.Identifier
                .Should()
                .Be("101");

            group.Identifiers.Count()
                .Should()
                .Be(2);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnProviderInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.UKPRN,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.Provider
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Provider 1");

            group.Identifier
                .Should()
                .Be("1001");

            group.Identifiers.Count()
                .Should()
                .Be(2);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.UKPRN)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.LACode)
                .Should()
                .Be(true);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnLocalAuthorityInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.LACode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.LocalAuthority
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Local Authority 1");

            group.Identifier
                .Should()
                .Be("101");

            group.Identifiers.Count()
                .Should()
                .Be(5);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.UKPRN)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.LACode)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.DfeNumber)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.CountryCode)
                .Should()
                .Be(true);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnParliamentaryConstituencyInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.ParliamentaryConstituency
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Bermondsey and Old Southwark");

            group.Identifier
                .Should()
                .Be("BOS");

            group.Identifiers.Count()
                .Should()
                .Be(5);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.LACode)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.RscRegionCode)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode)
                .Should()
                .Be(true);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.CountryCode)
                .Should()
                .Be(true);
        }


        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnDfeEstablshmentNummberInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.DfeEstablishmentNumber
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Name
                .Should()
                .Be("Dfe Establishment Number");

            group.Identifier
                .Should()
                .Be("Dfe Establishment Number");

            group.Identifiers
                .Should()
                .BeNull();
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnMiddleSuperInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.MiddleSuperOutputArea
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Middle Super Output Area 1");

            group.Identifier
                .Should()
                .Be("MSOA1");

            group.Identifiers.Any()
                .Should()
                .Be(false);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnCensusWardInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.CensusWardCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.CensusWard
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Census Ward 1");

            group.Identifier
                .Should()
                .Be("CW1");

            group.Identifiers.Any()
                .Should()
                .Be(false);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnDistrictCodeInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.DistrictCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.District
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("District 1");

            group.Identifier
                .Should()
                .Be("D1");

            group.Identifiers.Count()
                .Should()
                .Be(0);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnGovernmentOfficeRegionInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.GovernmentOfficeRegion
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Government Office Region 1");

            group.Identifier
                .Should()
                .Be("GOR1");

            group.Identifiers.Count()
                .Should()
                .Be(1);

            group.Identifiers.Any(_ => _.Type == Enums.OrganisationGroupTypeIdentifier.CountryCode)
                .Should()
                .Be(true);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnLowerSuperOutputAreaInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.LowerSuperOutputArea

            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Lower Super Output Area 1");

            group.Identifier
                .Should()
                .Be("LSOA1");

            group.Identifiers.Count()
                .Should()
                .Be(0);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnWardInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.WardCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.Ward
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Ward 1");

            group.Identifier
                .Should()
                .Be("W1");

            group.Identifiers.Count()
                .Should()
                .Be(0);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnRscRegionInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.RscRegionCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.RSCRegion
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Rsc Region 1");

            group.Identifier
                .Should()
                .Be("RSC1");

            group.Identifiers.Count()
                .Should()
                .Be(0);
        }

        [TestMethod]
        public async Task WhenLookingUpTargetOrganisationGroupBasedOnCountryInformation_ThenTargetOrganisationGroupReturned()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.CountryCode,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.Country
            };

            TargetOrganisationGroup group = await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                Common.ApiClient.Policies.Models.GroupingReason.Information,
                scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            group.Identifiers.Any();

            group.Name
                .Should()
                .Be("Country 1");

            group.Identifier
                .Should()
                .Be("C1");

            group.Identifiers.Count()
                .Should()
                .Be(0);
        }

        [TestMethod]
        public void WhenLookingUpTargetOrganisationGroupBasedOnUnknownGroupTypeInformation_ThenExceptionThrown()
        {
            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            OrganisationGroupLookupParameters organisationGroupLookupParameters = new OrganisationGroupLookupParameters
            {
                GroupTypeIdentifier = Common.ApiClient.Policies.Models.OrganisationGroupTypeIdentifier.UID,
                OrganisationGroupTypeCode = Common.ApiClient.Policies.Models.OrganisationGroupTypeCode.District
            };

            Func<Task<TargetOrganisationGroup>> action = async () => await _organisationGroupTargetProviderLookup.GetTargetProviderDetails(organisationGroupLookupParameters,
                    Common.ApiClient.Policies.Models.GroupingReason.Information,
                    scopedProviders.Where(_ => _.TrustStatus != TrustStatus.SupportedByAMultiAcademyTrust));

            action.Should().Throw<Exception>().WithMessage("Unable to resolve field to identifier value");
        }

        private Common.ApiClient.Models.ApiResponse<ProviderVersion> GetProviderVersion()
        {
            return new Common.ApiClient.Models.ApiResponse<ProviderVersion>(System.Net.HttpStatusCode.OK, new ProviderVersion { Providers = GenerateScopedProviders() });
        }

        private IEnumerable<Provider> GenerateScopedProviders()
        {
            List<Provider> providers = new List<Provider>();

            providers.Add(new Provider()
            {
                ProviderId = "Multi provider",
                Name = "Multi provider",
                TrustCode = "101",
                TrustName = "Academy Trust 1",
                TrustStatus = TrustStatus.SupportedByAMultiAcademyTrust,
                UKPRN = "1001",
                ProviderType = "Academy Trust",
                ProviderSubType = ""
            });

            providers.Add(new Provider()
            {
                ProviderId = "provider1",
                Name = "Provider 1",
                UKPRN = "1001",
                LACode = "101",
                Authority = "Local Authority 1",
                DfeEstablishmentNumber = "Dfe Establishment Number",
                TrustCode = "101",
                TrustName = "Academy Trust 1",
                ParliamentaryConstituencyCode = "BOS",
                ParliamentaryConstituencyName = "Bermondsey and Old Southwark",
                MiddleSuperOutputAreaCode = "MSOA1",
                MiddleSuperOutputAreaName = "Middle Super Output Area 1",
                CensusWardCode = "CW1",
                CensusWardName = "Census Ward 1",
                DistrictCode = "D1",
                DistrictName = "District 1",
                GovernmentOfficeRegionCode = "GOR1",
                GovernmentOfficeRegionName = "Government Office Region 1",
                LowerSuperOutputAreaCode = "LSOA1",
                LowerSuperOutputAreaName = "Lower Super Output Area 1",
                WardCode = "W1",
                WardName = "Ward 1",
                RscRegionCode = "RSC1",
                RscRegionName = "Rsc Region 1",
                CountryCode = "C1",
                CountryName = "Country 1",
                ProviderType = "Local Authority",
                ProviderSubType = "Local Authority"
            });

            providers.Add(new Provider()
            {
                ProviderId = "provider2",
                Name = "Provider 2",
                UKPRN = "1002",
                LACode = "101",
                Authority = "Local Authority 1",
                TrustCode = "101",
                TrustName = "Academy Trust 1",
                ParliamentaryConstituencyCode = "BOS",
                ParliamentaryConstituencyName = "Bermondsey and Old Southwark",
                MiddleSuperOutputAreaCode = "MSOA1",
                MiddleSuperOutputAreaName = "Middle Super Output Area 1",
                CensusWardCode = "CW1",
                CensusWardName = "Census Ward 1",
                DistrictCode = "D1",
                DistrictName = "District 1",
                GovernmentOfficeRegionCode = "GOR1",
                GovernmentOfficeRegionName = "Government Office Region 1",
                LowerSuperOutputAreaCode = "LSOA1",
                LowerSuperOutputAreaName = "Lower Super Output Area 1",
                WardCode = "W1",
                WardName = "Ward 1",
                RscRegionCode = "RSC1",
                RscRegionName = "Rsc Region 1",
                CountryCode = "C1",
                CountryName = "Country 1",
                ProviderType = "ProviderType",
                ProviderSubType = "ProviderSubType"
            });

            providers.Add(new Provider()
            {
                ProviderId = "provider3",
                Name = "Provider 3",
                UKPRN = "1003",
                LACode = "102",
                Authority = "Local Authority 2",
                TrustCode = "102",
                TrustName = "Academy Trust 2",
                TrustStatus = TrustStatus.SupportedByASingleAacademyTrust,
                ParliamentaryConstituencyCode = "CA",
                ParliamentaryConstituencyName = "Camden",
                MiddleSuperOutputAreaCode = "MSOA2",
                MiddleSuperOutputAreaName = "Middle Super Output Area 2",
                CensusWardCode = "CW2",
                CensusWardName = "Census Ward 2",
                DistrictCode = "D2",
                DistrictName = "District 2",
                GovernmentOfficeRegionCode = "GOR2",
                GovernmentOfficeRegionName = "Government Office Region 2",
                LowerSuperOutputAreaCode = "LSOA2",
                LowerSuperOutputAreaName = "Lower Super Output Area 2",
                WardCode = "W2",
                WardName = "Ward 2",
                RscRegionCode = "RSC2",
                RscRegionName = "Rsc Region 2",
                CountryCode = "C2",
                CountryName = "Country 2",
                ProviderType = "ProviderType",
                ProviderSubType = "ProviderSubType"
            });

            providers.Add(new Provider()
            {
                ProviderId = "provider4",
                Name = "Provider 3",
                UKPRN = "1004",
                LACode = "103",
                TrustCode = "103",
                TrustName = "Academy Trust 3",
                Authority = "Local Authority 3",
                DistrictCode = "D2",
                DistrictName = "District 2",
                ProviderType = "ProviderType",
                ProviderSubType = "ProviderSubType"
            });

            providers.Add(new Provider()
            {
                ProviderId = "provider5",
                Name = "Provider 5",
                UKPRN = "1004",
                LACode = "103",
                TrustCode = "103",
                TrustName = "Academy Trust 3",
                Authority = "Local Authority 3",
                DistrictCode = "D2",
                DistrictName = "District 2",
                ProviderType = "ProviderType2",
                ProviderSubType = "ProviderSubType2"
            });

            return providers;
        }
    }
}
