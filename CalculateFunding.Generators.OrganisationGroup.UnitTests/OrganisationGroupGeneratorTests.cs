using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Generators.OrganisationGroup.Interfaces;
using CalculateFunding.Generators.OrganisationGroup.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CalculateFunding.Generators.OrganisationGroup.UnitTests
{
    [TestClass]
    public class OrganisationGroupGeneratorTests
    {
        private IOrganisationGroupTargetProviderLookup _organisationGroupTargetProviderLookup;
        private OrganisationGroupGenerator _generator;
        private string _providerVersionId;

        [TestInitialize]
        public void Setup()
        {
            _organisationGroupTargetProviderLookup = Substitute.For<IOrganisationGroupTargetProviderLookup>();
            _generator = new OrganisationGroupGenerator(_organisationGroupTargetProviderLookup);
            _providerVersionId = "test-providers";
        }

        [TestMethod]
        public async Task WhenProducingPaymentForASpecForLocalAuthority_ThenOrganisationGroupsAreCreated()
        {
            // Arrange
            FundingConfiguration fundingConfiguration = new FundingConfiguration()
            {
                OrganisationGroupings = new List<OrganisationGroupingConfiguration>()
                {
                    new OrganisationGroupingConfiguration()
                    {
                        GroupingReason = GroupingReason.Payment,
                        GroupTypeClassification = OrganisationGroupTypeClassification.LegalEntity,
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.UKPRN,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.LocalAuthority,
                    },
                },
            };

            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            TargetOrganisationGroup la1 = new TargetOrganisationGroup()
            {
                Identifier = "7777",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is("101"), Arg.Is(GroupingReason.Payment), Arg.Is(OrganisationGroupTypeCode.LocalAuthority), Arg.Is(OrganisationGroupTypeIdentifier.UKPRN), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>())
                .Returns(la1);

            TargetOrganisationGroup la2 = new TargetOrganisationGroup()
            {
                Identifier = "8888",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is("102"), Arg.Is(GroupingReason.Payment), Arg.Is(OrganisationGroupTypeCode.LocalAuthority), Arg.Is(OrganisationGroupTypeIdentifier.UKPRN), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>())
                .Returns(la2);

            TargetOrganisationGroup la3 = new TargetOrganisationGroup()
            {
                Identifier = "9999",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 3",
            };

            _organisationGroupTargetProviderLookup
               .GetTargetProviderDetails(Arg.Is("103"), Arg.Is(GroupingReason.Payment), Arg.Is(OrganisationGroupTypeCode.LocalAuthority), Arg.Is(OrganisationGroupTypeIdentifier.UKPRN), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>())
               .Returns(la3);

            // Act
            IEnumerable<Models.OrganisationGroupResult> result = await _generator.GenerateOrganisationGroup(fundingConfiguration, scopedProviders, _providerVersionId);

            // Assert
            result
                .Should()
                .NotBeNull();

            List<OrganisationGroupResult> expectedResult = new List<OrganisationGroupResult>()
            {
                new OrganisationGroupResult()
                {
                    Name = "Local Authority 1",
                    SearchableName = "LocalAuthority1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.UKPRN,
                    IdentifierValue = "7777",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "101")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Local Authority 2",
                    SearchableName = "LocalAuthority2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.UKPRN,
                    IdentifierValue = "8888",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "102")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Local Authority 3",
                    SearchableName = "LocalAuthority3",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.UKPRN,
                    IdentifierValue = "9999",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "103")),
                },
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
               .GetTargetProviderDetails(Arg.Is("101"), Arg.Is(GroupingReason.Payment), Arg.Is(OrganisationGroupTypeCode.LocalAuthority), Arg.Is(OrganisationGroupTypeIdentifier.UKPRN), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>());

            await _organisationGroupTargetProviderLookup
                .Received(1)
               .GetTargetProviderDetails(Arg.Is("102"), Arg.Is(GroupingReason.Payment), Arg.Is(OrganisationGroupTypeCode.LocalAuthority), Arg.Is(OrganisationGroupTypeIdentifier.UKPRN), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>());

            await _organisationGroupTargetProviderLookup
                .Received(1)
               .GetTargetProviderDetails(Arg.Is("103"), Arg.Is(GroupingReason.Payment), Arg.Is(OrganisationGroupTypeCode.LocalAuthority), Arg.Is(OrganisationGroupTypeIdentifier.UKPRN), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>());
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByParlimentaryConsituency_ThenOrganisationGroupsAreCreated()
        {
            // Arrange
            FundingConfiguration fundingConfiguration = new FundingConfiguration()
            {
                OrganisationGroupings = new List<OrganisationGroupingConfiguration>()
                {
                    new OrganisationGroupingConfiguration()
                    {
                        GroupingReason = GroupingReason.Information,
                        GroupTypeClassification = OrganisationGroupTypeClassification.GeographicalBoundary,
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.ParliamentaryConstituency,
                    },
                },
            };

            TargetOrganisationGroup bos = new TargetOrganisationGroup()
            {
                Identifier = "BOS",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Bermondsey and Old Southwark",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is("BOS"), Arg.Is(GroupingReason.Information), Arg.Is(OrganisationGroupTypeCode.ParliamentaryConstituency), Arg.Is(OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>())
                .Returns(bos);

            TargetOrganisationGroup camden = new TargetOrganisationGroup()
            {
                Identifier = "CA",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Camden",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is("CA"), Arg.Is(GroupingReason.Information), Arg.Is(OrganisationGroupTypeCode.ParliamentaryConstituency), Arg.Is(OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>())
                .Returns(camden);


            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            // Act
            IEnumerable<Models.OrganisationGroupResult> result = await _generator.GenerateOrganisationGroup(fundingConfiguration, scopedProviders, _providerVersionId);

            // Assert
            result
                .Should()
                .NotBeNull();

            List<OrganisationGroupResult> expectedResult = new List<OrganisationGroupResult>()
            {
                new OrganisationGroupResult()
                {
                    Name = "Bermondsey and Old Southwark",
                    SearchableName = "BermondseyandOldSouthwark",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.ParliamentaryConstituency,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode,
                    IdentifierValue = "BOS",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.ParliamentaryConstituencyCode == "BOS")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Camden",
                    SearchableName = "Camden",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.ParliamentaryConstituency,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode,
                    IdentifierValue = "CA",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.ParliamentaryConstituencyCode == "CA")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
               .GetTargetProviderDetails(Arg.Is("BOS"), Arg.Is(GroupingReason.Information), Arg.Is(OrganisationGroupTypeCode.ParliamentaryConstituency), Arg.Is(OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>());

            await _organisationGroupTargetProviderLookup
                .Received(1)
               .GetTargetProviderDetails(Arg.Is("CA"), Arg.Is(GroupingReason.Information), Arg.Is(OrganisationGroupTypeCode.ParliamentaryConstituency), Arg.Is(OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(_providerVersionId), Arg.Any<IEnumerable<Provider>>());
        }

        private IEnumerable<Provider> GenerateScopedProviders()
        {
            List<Provider> providers = new List<Provider>();

            providers.Add(new Provider()
            {
                ProviderId = "provider1",
                Name = "Provider 1",
                UKPRN = "1001",
                LACode = "101",
                Authority = "Local Authority 1",
                ParliamentaryConstituencyCode = "BOS",
                ParliamentaryConstituencyName = "Bermondsey and Old Southwark",
            });

            providers.Add(new Provider()
            {
                ProviderId = "provider2",
                Name = "Provider 2",
                UKPRN = "1002",
                LACode = "101",
                Authority = "Local Authority 1",
                ParliamentaryConstituencyCode = "BOS",
                ParliamentaryConstituencyName = "Bermondsey and Old Southwark",
            });

            providers.Add(new Provider()
            {
                ProviderId = "provider3",
                Name = "Provider 3",
                UKPRN = "1003",
                LACode = "102",
                Authority = "Local Authority 2",
                ParliamentaryConstituencyCode = "CA",
                ParliamentaryConstituencyName = "Camden",
            });

            providers.Add(new Provider()
            {
                ProviderId = "provider4",
                Name = "Provider 3",
                UKPRN = "1004",
                LACode = "103",
                Authority = "Local Authority 3",
            });

            return providers;
        }
    }
}
