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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "101" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "102" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "103" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.ProviderVersionId == _providerVersionId), 
               Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Local_Authority_1",
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
                    SearchableName = "Local_Authority_2",
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
                    SearchableName = "Local_Authority_3",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "101" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "102" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.ProviderVersionId == _providerVersionId),
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "103" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.LocalAuthority && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "101" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "102" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "103" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Academy_Trust_1",
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
                    SearchableName = "Academy_Trust_2",
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
                    SearchableName = "Academy_Trust_3",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "101" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));


            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "102" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));


            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "103" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.AcademyTrust && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "1001" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "1002" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "1003" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "1004" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.ProviderVersionId == _providerVersionId), 
               Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Provider_1",
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
                    SearchableName = "Provider_2",
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
                    SearchableName = "Provider_3",
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
                    SearchableName = "Provider_4",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "1001" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "1002" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "1003" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.IdentifierValue == "1004" && _.OrganisationGroupTypeCode == OrganisationGroupTypeCode.Provider && _.ProviderVersionId == _providerVersionId), 
                Arg.Is(GroupingReason.Payment), Arg.Any<IEnumerable<Provider>>(), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().ParliamentaryConstituencyCode == "BOS"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().ParliamentaryConstituencyCode == "CA"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Bermondsey_and_Old_Southwark",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().ParliamentaryConstituencyCode == "CA"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.ParliamentaryConstituencyCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().ParliamentaryConstituencyCode == "BOS"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().MiddleSuperOutputAreaCode == "MSOA1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().MiddleSuperOutputAreaCode == "MSOA2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Middle_Super_Output_Area_1",
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
                    SearchableName = "Middle_Super_Output_Area_2",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().MiddleSuperOutputAreaCode == "MSOA1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));


            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.MiddleSuperOutputAreaCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().MiddleSuperOutputAreaCode == "MSOA2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.CensusWardCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().CensusWardCode == "CW1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.CensusWardCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().CensusWardCode == "CW2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Census_Ward_1",
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
                    SearchableName = "Census_Ward_2",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.CensusWardCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().CensusWardCode == "CW1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.CensusWardCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().CensusWardCode == "CW2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.DistrictCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().DistrictCode == "D1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.DistrictCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().DistrictCode == "D2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "District_1",
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
                    SearchableName = "District_2",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.DistrictCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().DistrictCode == "D1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.DistrictCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().DistrictCode == "D2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().GovernmentOfficeRegionCode == "GOR1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().GovernmentOfficeRegionCode == "GOR2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Government_Office_Region_1",
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
                    SearchableName = "Government_Office_Region_2",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode), 
                Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().GovernmentOfficeRegionCode == "GOR1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.GovernmentOfficeRegionCode), 
                Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().GovernmentOfficeRegionCode == "GOR2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LowerSuperOutputAreaCode == "LSOA1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LowerSuperOutputAreaCode == "LSOA2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Lower_Super_Output_Area_1",
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
                    SearchableName = "Lower_Super_Output_Area_2",
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
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode), Arg.Is(GroupingReason.Information), 
               Arg.Is<IEnumerable<Provider>>(_ => _.First().LowerSuperOutputAreaCode == "LSOA1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LowerSuperOutputAreaCode), Arg.Is(GroupingReason.Information), 
               Arg.Is<IEnumerable<Provider>>(_ => _.First().LowerSuperOutputAreaCode == "LSOA2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.WardCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().WardCode == "W1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.WardCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().WardCode == "W2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Ward_1",
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
                    SearchableName = "Ward_2",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.WardCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().WardCode == "W1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.WardCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().WardCode == "W2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.RscRegionCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().RscRegionCode == "RSC1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.RscRegionCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().RscRegionCode == "RSC2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Rsc_Region_1",
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
                    SearchableName = "Rsc_Region_2",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.RscRegionCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().RscRegionCode == "RSC1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.RscRegionCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().RscRegionCode == "RSC2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
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
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.CountryCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().CountryCode == "C1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.CountryCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().CountryCode == "C2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
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
                    SearchableName = "Country_1",
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
                    SearchableName = "Country_2",
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
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.CountryCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().CountryCode == "C1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.CountryCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().CountryCode == "C2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByLocalAuthorityClassification_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.LocalAuthority,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
            };

            TargetOrganisationGroup lac1 = new TargetOrganisationGroup()
            {
                Identifier = "LAC1",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "LocalGovernmentGroupTypeName1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LocalGovernmentGroupTypeName == "LocalGovernmentGroupTypeName1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
                .Returns(lac1);

            TargetOrganisationGroup lac2 = new TargetOrganisationGroup()
            {
                Identifier = "LAC2",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "LocalGovernmentGroupTypeName2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LocalGovernmentGroupTypeName == "LocalGovernmentGroupTypeName2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
                .Returns(lac2);


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
                    Name = "LocalGovernmentGroupTypeName1",
                    SearchableName = "LocalGovernmentGroupTypeName1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "LAC1",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LocalGovernmentGroupTypeName == "LocalGovernmentGroupTypeName1")),
                },
                new OrganisationGroupResult()
                {
                    Name = "LocalGovernmentGroupTypeName2",
                    SearchableName = "LocalGovernmentGroupTypeName2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "LAC2",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LocalGovernmentGroupTypeName == "LocalGovernmentGroupTypeName2")),
                }
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode), 
                Arg.Is(GroupingReason.Information), Arg.Is<IEnumerable<Provider>>(_ => _.First().LocalGovernmentGroupTypeName == "LocalGovernmentGroupTypeName1"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LocalAuthorityClassificationTypeCode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LocalGovernmentGroupTypeName == "LocalGovernmentGroupTypeName2"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByLACode_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.LACode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.LocalAuthority,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
            };

            TargetOrganisationGroup c1 = new TargetOrganisationGroup()
            {
                Identifier = "101",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "101"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
                .Returns(c1);

            TargetOrganisationGroup c2 = new TargetOrganisationGroup()
            {
                Identifier = "102",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "102"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
                .Returns(c2);

            TargetOrganisationGroup c3 = new TargetOrganisationGroup()
            {
                Identifier = "103",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 3",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "103"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
                .Returns(c3);


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
                    Name = "Local Authority 1",
                    SearchableName = "Local_Authority_1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LACode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "101",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "101" && p.ProviderType == "ProviderType")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Local Authority 2",
                    SearchableName = "Local_Authority_2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LACode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "102",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "102" && p.ProviderType == "ProviderType")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Local Authority 3",
                    SearchableName = "Local_Authority_3",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LACode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "103",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "103" && p.ProviderType == "ProviderType")),
                },
            };

            result
                .Should()
                .BeEquivalentTo(expectedResult);

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "101"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
                .Received(1)
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "102"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));

            await _organisationGroupTargetProviderLookup
               .Received(1)
               .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
               Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "103"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider));
        }

        [TestMethod]
        public async Task WhenCreatingInformationOrganisationGroupsByLACodeAndProvidersAreSuccessfullyExcluded_ThenOrganisationGroupsAreCreated()
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
                        GroupTypeIdentifier = OrganisationGroupTypeIdentifier.LACode,
                        OrganisationGroupTypeCode = OrganisationGroupTypeCode.LocalAuthority,
                        ProviderTypeMatch = new List<ProviderTypeMatch> { new ProviderTypeMatch { ProviderType = "ProviderType", ProviderSubtype = "ProviderSubType" } }
                    },
                },
                PaymentOrganisationSource = PaymentOrganisationSource.PaymentOrganisationAsProvider
            };

            TargetOrganisationGroup c1 = new TargetOrganisationGroup()
            {
                Identifier = "101",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 1",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "101"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
                .Returns(c1);

            TargetOrganisationGroup c2 = new TargetOrganisationGroup()
            {
                Identifier = "102",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 2",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "102"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
                .Returns(c2);

            TargetOrganisationGroup c3 = new TargetOrganisationGroup()
            {
                Identifier = "103",
                Identifiers = new List<OrganisationIdentifier>()
                {
                },
                Name = "Local Authority 3",
            };

            _organisationGroupTargetProviderLookup
                .GetTargetProviderDetails(Arg.Is<OrganisationGroupLookupParameters>(_ => _.GroupTypeIdentifier == OrganisationGroupTypeIdentifier.LACode), Arg.Is(GroupingReason.Information), 
                Arg.Is<IEnumerable<Provider>>(_ => _.First().LACode == "103"), Arg.Is<PaymentOrganisationSource>(_ => _ == PaymentOrganisationSource.PaymentOrganisationAsProvider))
                .Returns(c3);

            Provider excludedProvider = new Provider()
            {
                ProviderId = "providerExcluded",
                Name = "Provider Excluded",
                UKPRN = "10066",
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
                ProviderType = "ProviderTypeExcluded",
                ProviderSubType = "ProviderTypeExcluded",
                LocalGovernmentGroupTypeCode = "LocalGovernmentGroupTypeCode1",
                LocalGovernmentGroupTypeName = "LocalGovernmentGroupTypeName1",
            };

            IEnumerable<Provider> scopedProviders = GenerateScopedProviders()
                .Concat(new[] { excludedProvider });


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
                    SearchableName = "Local_Authority_1",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LACode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "101",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "101" && p.ProviderType == "ProviderType")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Local Authority 2",
                    SearchableName = "Local_Authority_2",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LACode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "102",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "102" && p.ProviderType == "ProviderType")),
                },
                new OrganisationGroupResult()
                {
                    Name = "Local Authority 3",
                    SearchableName = "Local_Authority_3",
                    GroupTypeClassification = Enums.OrganisationGroupTypeClassification.GeographicalBoundary,
                    GroupTypeCode = Enums.OrganisationGroupTypeCode.LocalAuthority,
                    GroupTypeIdentifier = Enums.OrganisationGroupTypeIdentifier.LACode,
                    GroupReason = Enums.OrganisationGroupingReason.Information,
                    IdentifierValue = "103",
                    Identifiers = new List<OrganisationIdentifier>(),
                    Providers = new List<Provider>(scopedProviders.Where(p=>p.LACode == "103" && p.ProviderType == "ProviderType")),
                },
            };

            result
                .Should()
                    .BeEquivalentTo(expectedResult);

            OrganisationGroupResult laForExcludedProvider = result.Single(l => l.IdentifierValue == "101");
            laForExcludedProvider
                .Providers
                .Should()
                .NotContain(excludedProvider);
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
                ProviderSubType = "ProviderSubType",
                LocalGovernmentGroupTypeCode = "LocalGovernmentGroupTypeCode1",
                LocalGovernmentGroupTypeName = "LocalGovernmentGroupTypeName1"
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
                ProviderSubType = "ProviderSubType",
                LocalGovernmentGroupTypeCode = "LocalGovernmentGroupTypeCode1",
                LocalGovernmentGroupTypeName = "LocalGovernmentGroupTypeName1"
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
                ProviderSubType = "ProviderSubType",
                LocalGovernmentGroupTypeCode = "LocalGovernmentGroupTypeCode2",
                LocalGovernmentGroupTypeName = "LocalGovernmentGroupTypeName2"
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
