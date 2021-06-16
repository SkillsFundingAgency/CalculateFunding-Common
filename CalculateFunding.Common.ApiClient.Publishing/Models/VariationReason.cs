using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VariationReason
    {
        [EnumMember(Value = "AuthorityFieldUpdated")]
        AuthorityFieldUpdated,

        [EnumMember(Value = "EstablishmentNumberFieldUpdated")]
        EstablishmentNumberFieldUpdated,

        [EnumMember(Value = "DfeEstablishmentNumberFieldUpdated")]
        DfeEstablishmentNumberFieldUpdated,

        [EnumMember(Value = "NameFieldUpdated")]
        NameFieldUpdated,

        [EnumMember(Value = "LACodeFieldUpdated")]
        LACodeFieldUpdated,

        [EnumMember(Value = "LegalNameFieldUpdated")]
        LegalNameFieldUpdated,

        [EnumMember(Value = "TrustCodeFieldUpdated")]
        TrustCodeFieldUpdated,

        [EnumMember(Value = "FundingUpdated")]
        FundingUpdated,

        [EnumMember(Value = "ProfilingUpdated")]
        ProfilingUpdated,

        [EnumMember(Value = "URNFieldUpdated")]
        URNFieldUpdated,

        [EnumMember(Value = "CompaniesHouseNumberFieldUpdated")]
        CompaniesHouseNumberFieldUpdated,

        [EnumMember(Value = "GroupIDFieldUpdated")]
        GroupIDFieldUpdated,

        [EnumMember(Value = "RSCRegionCodeFieldUpdated")]
        RSCRegionCodeFieldUpdated,

        [EnumMember(Value = "RSCRegionNameFieldUpdated")]
        RSCRegionNameFieldUpdated,

        [EnumMember(Value = "GovernmentOfficeRegionCodeFieldUpdated")]
        GovernmentOfficeRegionCodeFieldUpdated,

        [EnumMember(Value = "GovernmentOfficeRegionNameFieldUpdated")]
        GovernmentOfficeRegionNameFieldUpdated,

        [EnumMember(Value = "DistrictCodeFieldUpdated")]
        DistrictCodeFieldUpdated,

        [EnumMember(Value = "DistrictNameFieldUpdated")]
        DistrictNameFieldUpdated,

        [EnumMember(Value = "WardCodeFieldUpdated")]
        WardCodeFieldUpdated,

        [EnumMember(Value = "WardNameFieldUpdated")]
        WardNameFieldUpdated,

        [EnumMember(Value = "CensusWardCodeFieldUpdated")]
        CensusWardCodeFieldUpdated,

        [EnumMember(Value = "CensusWardNameFieldUpdated")]
        CensusWardNameFieldUpdated,

        [EnumMember(Value = "MiddleSuperOutputAreaCodeFieldUpdated")]
        MiddleSuperOutputAreaCodeFieldUpdated,

        [EnumMember(Value = "MiddleSuperOutputAreaNameFieldUpdated")]
        MiddleSuperOutputAreaNameFieldUpdated,

        [EnumMember(Value = "LowerSuperOutputAreaCodeFieldUpdated")]
        LowerSuperOutputAreaCodeFieldUpdated,

        [EnumMember(Value = "LowerSuperOutputAreaNameFieldUpdated")]
        LowerSuperOutputAreaNameFieldUpdated,

        [EnumMember(Value = "ParliamentaryConstituencyCodeFieldUpdated")]
        ParliamentaryConstituencyCodeFieldUpdated,

        [EnumMember(Value = "ParliamentaryConstituencyNameFieldUpdated")]
        ParliamentaryConstituencyNameFieldUpdated,

        [EnumMember(Value = "CountryCodeFieldUpdated")]
        CountryCodeFieldUpdated,

        [EnumMember(Value = "CountryNameFieldUpdated")]
        CountryNameFieldUpdated,

        [EnumMember(Value = nameof(PaymentOrganisationIdentifierFieldUpdated))]
        PaymentOrganisationIdentifierFieldUpdated,

        [EnumMember(Value = nameof(PaymentOrganisationNameFieldUpdated))]
        PaymentOrganisationNameFieldUpdated,

        [EnumMember(Value = nameof(DateOpenedFieldUpdated))]
        DateOpenedFieldUpdated,

        [EnumMember(Value = nameof(DateClosedFieldUpdated))]
        DateClosedFieldUpdated,

        [EnumMember(Value = nameof(ProviderStatusFieldUpdated))]
        ProviderStatusFieldUpdated,

        [EnumMember(Value = nameof(PhaseOfEducationFieldUpdated))]
        PhaseOfEducationFieldUpdated,

        [EnumMember(Value = nameof(ReasonEstablishmentOpenedFieldUpdated))]
        ReasonEstablishmentOpenedFieldUpdated,

        [EnumMember(Value = nameof(ReasonEstablishmentClosedFieldUpdated))]
        ReasonEstablishmentClosedFieldUpdated,

        [EnumMember(Value = nameof(TrustStatusFieldUpdated))]
        TrustStatusFieldUpdated,

        [EnumMember(Value = nameof(TownFieldUpdated))]
        TownFieldUpdated,

        [EnumMember(Value = nameof(PostcodeFieldUpdated))]
        PostcodeFieldUpdated,

        [EnumMember(Value = nameof(TemplateUpdated))]
        TemplateUpdated,

        [EnumMember(Value = nameof(FundingSchemaUpdated))]
        FundingSchemaUpdated,

        [EnumMember(Value = nameof(DistributionProfileUpdated))]
        DistributionProfileUpdated,

        [EnumMember(Value = nameof(IndicativeToLive))]
        IndicativeToLive,
    }
}
