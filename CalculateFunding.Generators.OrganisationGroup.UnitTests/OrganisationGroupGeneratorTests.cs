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
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.LocalAuthority
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "101" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "102" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
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
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "103" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
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
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
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
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
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
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "101" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "102" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "103" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());
        }

        [TestMethod]
        public async Task WhenProducingPaymentForASpecForAcademyTrust_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.AcademyTrustCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.AcademyTrust,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            TargetOrganisationGroup at1 = new TargetOrganisationGroup()
            {
                Identifier = "7777",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Academy Trust 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "101" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
                .Returns(at1);

            TargetOrganisationGroup at2 = new TargetOrganisationGroup()
            {
                Identifier = "8888",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Academy Trust 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "102" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
                .Returns(at2);

            TargetOrganisationGroup at3 = new TargetOrganisationGroup()
            {
                Identifier = "9999",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Academy Trust 3",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "103" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
                .Returns(at3);

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
                    Name = "Academy Trust 1",
                    SearchableName = "AcademyTrust1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.AcademyTrust,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.AcademyTrustCode,
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
                    IdentifierValue = "7777",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.TrustCode == "101")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Academy Trust 2",
                    SearchableName = "AcademyTrust2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.AcademyTrust,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.AcademyTrustCode,
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
                    IdentifierValue = "8888",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.TrustCode == "102")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Academy Trust 3",
                    SearchableName = "AcademyTrust3",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.AcademyTrust,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.AcademyTrustCode,
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
                    IdentifierValue = "9999",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.TrustCode == "103" && p.ProviderType == "ProviderType")),
                },
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "101" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());


            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "102" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());


            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "103" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());
        }

        [TestMethod]
        public async Task WhenProducingPaymentForASpecForProvider_ThenOrganisationGroupsAreCreated()
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
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.Provider,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            IEnumerable<Provider> scopedProviders = GenerateScopedProviders();

            TargetOrganisationGroup p1 = new TargetOrganisationGroup()
            {
                Identifier = "6666",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Provider 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "1001" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
                .Returns(p1);

            TargetOrganisationGroup p2 = new TargetOrganisationGroup()
            {
                Identifier = "7777",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Provider 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "1002" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
                .Returns(p2);

            TargetOrganisationGroup p3 = new TargetOrganisationGroup()
            {
                Identifier = "8888",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Provider 3",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "1003" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
                .Returns(p3);

            TargetOrganisationGroup p4 = new TargetOrganisationGroup()
            {
                Identifier = "9999",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Provider 4",
            };

            _organisationGroupTargetProviderLookup
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "1004" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>())
               .Returns(p4);

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
                    Name = "Provider 1",
                    SearchableName = "Provider1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.Provider,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.UKPRN,
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
                    IdentifierValue = "6666",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.UKPRN == "1001")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Provider 2",
                    SearchableName = "Provider2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.Provider,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.UKPRN,
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
                    IdentifierValue = "7777",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.UKPRN == "1002")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Provider 3",
                    SearchableName = "Provider3",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.Provider,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.UKPRN,
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
                    IdentifierValue = "8888",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.UKPRN == "1003")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Provider 4",
                    SearchableName = "Provider4",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.LegalEntity,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.Provider,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.UKPRN,
                    GroupReason = Enums.OrganisationGroupingReason.Payment,
                    IdentifierValue = "9999",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.UKPRN == "1004" && p.ProviderType == "ProviderType")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "1001" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "1002" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "1003" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.identifierValue == "1004" && _.organisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.providerVersionId == _providerVersionId), Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>());
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
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().ParliamentaryConstituencyCode == "BOS"))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().ParliamentaryConstituencyCode == "CA"))
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
                    GroupReason = Enums.OrganisationGroupingReason.Information,
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
                    GroupReason = Enums.OrganisationGroupingReason.Information,
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().ParliamentaryConstituencyCode == "CA"));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().ParliamentaryConstituencyCode == "BOS"));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByMiddleSuperOutputArea_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.MiddleSuperOutputArea,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            TargetOrganisationGroup msoa1 = new TargetOrganisationGroup()
            {
                Identifier = "MSOA1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Middle Super Output Area 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().MiddleSuperOutputAreaCode == "MSOA1"))
                .Returns(msoa1);

            TargetOrganisationGroup msoa2 = new TargetOrganisationGroup()
            {
                Identifier = "MSOA2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Middle Super Output Area 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().MiddleSuperOutputAreaCode == "MSOA2"))
                .Returns(msoa2);


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
                    Name = "Middle Super Output Area 1",
                    SearchableName = "MiddleSuperOutputArea1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.MiddleSuperOutputArea,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "MSOA1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.MiddleSuperOutputAreaCode == "MSOA1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Middle Super Output Area 2",
                    SearchableName = "MiddleSuperOutputArea2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.MiddleSuperOutputArea,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "MSOA2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.MiddleSuperOutputAreaCode == "MSOA2")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().MiddleSuperOutputAreaCode == "MSOA1"));


            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().MiddleSuperOutputAreaCode == "MSOA2"));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByCensusWard_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.CensusWardCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.CensusWard,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            TargetOrganisationGroup cw1 = new TargetOrganisationGroup()
            {
                Identifier = "CW1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Census Ward 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.CensusWardCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().CensusWardCode == "CW1"))
                .Returns(cw1);

            TargetOrganisationGroup cw2 = new TargetOrganisationGroup()
            {
                Identifier = "CW2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Census Ward 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.CensusWardCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().CensusWardCode == "CW2"))
                .Returns(cw2);


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
                    Name = "Census Ward 1",
                    SearchableName = "CensusWard1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.CensusWard,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.CensusWardCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "CW1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.CensusWardCode == "CW1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Census Ward 2",
                    SearchableName = "CensusWard2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.CensusWard,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.CensusWardCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "CW2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.CensusWardCode == "CW2")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.CensusWardCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().CensusWardCode == "CW1"));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.CensusWardCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().CensusWardCode == "CW2"));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByDistrict_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.DistrictCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.District,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            TargetOrganisationGroup d1 = new TargetOrganisationGroup()
            {
                Identifier = "D1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "District 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.DistrictCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().DistrictCode == "D1"))
                .Returns(d1);

            TargetOrganisationGroup d2 = new TargetOrganisationGroup()
            {
                Identifier = "D2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "District 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.DistrictCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().DistrictCode == "D2"))
                .Returns(d2);


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
                    Name = "District 1",
                    SearchableName = "District1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.District,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.DistrictCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "D1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.DistrictCode == "D1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "District 2",
                    SearchableName = "District2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.District,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.DistrictCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "D2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.DistrictCode == "D2" && p.ProviderType == "ProviderType")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.DistrictCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().DistrictCode == "D1"));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.DistrictCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().DistrictCode == "D2"));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByGovernmentOfficeRegion_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.GovernmentOfficeRegion,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            TargetOrganisationGroup gor1 = new TargetOrganisationGroup()
            {
                Identifier = "GOR1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Government Office Region 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().GovernmentOfficeRegionCode == "GOR1"))
                .Returns(gor1);

            TargetOrganisationGroup gor2 = new TargetOrganisationGroup()
            {
                Identifier = "GOR2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Government Office Region 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().GovernmentOfficeRegionCode == "GOR2"))
                .Returns(gor2);


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
                    Name = "Government Office Region 1",
                    SearchableName = "GovernmentOfficeRegion1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.GovernmentOfficeRegion,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "GOR1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.GovernmentOfficeRegionCode == "GOR1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Government Office Region 2",
                    SearchableName = "GovernmentOfficeRegion2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.GovernmentOfficeRegion,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "GOR2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.GovernmentOfficeRegionCode == "GOR2")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().GovernmentOfficeRegionCode == "GOR1"));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().GovernmentOfficeRegionCode == "GOR2"));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByGovernmentLowerSuperOutputArea_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.LowerSuperOutputArea,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            TargetOrganisationGroup lsoa1 = new TargetOrganisationGroup()
            {
                Identifier = "LSOA1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Lower Super Output Area 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().LowerSuperOutputAreaCode == "LSOA1"))
                .Returns(lsoa1);

            TargetOrganisationGroup lsoa2 = new TargetOrganisationGroup()
            {
                Identifier = "LSOA2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Lower Super Output Area 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().LowerSuperOutputAreaCode == "LSOA2"))
                .Returns(lsoa2);


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
                    Name = "Lower Super Output Area 1",
                    SearchableName = "LowerSuperOutputArea1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LowerSuperOutputArea,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "LSOA1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LowerSuperOutputAreaCode == "LSOA1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Lower Super Output Area 2",
                    SearchableName = "LowerSuperOutputArea2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LowerSuperOutputArea,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "LSOA2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LowerSuperOutputAreaCode == "LSOA2")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().LowerSuperOutputAreaCode == "LSOA1"));

            await _organisationGroupTargetProviderLookup
                .Received(1)
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().LowerSuperOutputAreaCode == "LSOA2"));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByGovernmentWard_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.WardCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.Ward,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            TargetOrganisationGroup w1 = new TargetOrganisationGroup()
            {
                Identifier = "W1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Ward 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.WardCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().WardCode == "W1"))
                .Returns(w1);

            TargetOrganisationGroup w2 = new TargetOrganisationGroup()
            {
                Identifier = "W2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Ward 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.WardCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().WardCode == "W2"))
                .Returns(w2);


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
                    Name = "Ward 1",
                    SearchableName = "Ward1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.Ward,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.WardCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "W1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.WardCode == "W1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Ward 2",
                    SearchableName = "Ward2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.Ward,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.WardCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "W2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.WardCode == "W2")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.WardCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().WardCode == "W1"));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.WardCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().WardCode == "W2"));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByRscRegion_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.RscRegionCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.RSCRegion,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            TargetOrganisationGroup rsc1 = new TargetOrganisationGroup()
            {
                Identifier = "RSC1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Rsc Region 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.RscRegionCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().RscRegionCode == "RSC1"))
                .Returns(rsc1);

            TargetOrganisationGroup rsc2 = new TargetOrganisationGroup()
            {
                Identifier = "RSC2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Rsc Region 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.RscRegionCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().RscRegionCode == "RSC2"))
                .Returns(rsc2);


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
                    Name = "Rsc Region 1",
                    SearchableName = "RscRegion1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.RSCRegion,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.RscRegionCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "RSC1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.RscRegionCode == "RSC1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Rsc Region 2",
                    SearchableName = "RscRegion2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.RSCRegion,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.RscRegionCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "RSC2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.RscRegionCode == "RSC2")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.RscRegionCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().RscRegionCode == "RSC1"));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.RscRegionCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().RscRegionCode == "RSC2"));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByCountry_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.CountryCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.Country,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
            };

            TargetOrganisationGroup c1 = new TargetOrganisationGroup()
            {
                Identifier = "C1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Country 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.CountryCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().CountryCode == "C1"))
                .Returns(c1);

            TargetOrganisationGroup c2 = new TargetOrganisationGroup()
            {
                Identifier = "C2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Country 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.CountryCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().CountryCode == "C2"))
                .Returns(c2);


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
                    Name = "Country 1",
                    SearchableName = "Country1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.Country,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.CountryCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "C1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.CountryCode == "C1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Country 2",
                    SearchableName = "Country2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.Country,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.CountryCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "C2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.CountryCode == "C2")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.CountryCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().CountryCode == "C1"));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.groupTypeIdentifier == OrganisationGroupTypeIdentifier.CountryCode), Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().CountryCode == "C2"));
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
