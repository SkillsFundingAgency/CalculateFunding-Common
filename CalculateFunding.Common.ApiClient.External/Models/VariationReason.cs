﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.External.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VariationReason
    {
        AuthorityFieldUpdated,

        EstablishmentNumberFieldUpdated,

        DfeEstablishmentNumberFieldUpdated,

        NameFieldUpdated,

        LACodeFieldUpdated,

        LAOrgFieldUpdated,

        LegalNameFieldUpdated,

        TrustCodeFieldUpdated,

        FundingUpdated,

        ProfilingUpdated,

        URNFieldUpdated,

        CompaniesHouseNumberFieldUpdated,

        GroupIDFieldUpdated,

        RSCRegionCodeFieldUpdated,

        RSCRegionNameFieldUpdated,

        GovernmentOfficeRegionCodeFieldUpdated,

        GovernmentOfficeRegionNameFieldUpdated,

        DistrictCodeFieldUpdated,

        DistrictNameFieldUpdated,

        WardCodeFieldUpdated,

        WardNameFieldUpdated,

        CensusWardCodeFieldUpdated,

        CensusWardNameFieldUpdated,

        MiddleSuperOutputAreaCodeFieldUpdated,

        MiddleSuperOutputAreaNameFieldUpdated,

        LowerSuperOutputAreaCodeFieldUpdated,

        LowerSuperOutputAreaNameFieldUpdated,

        ParliamentaryConstituencyCodeFieldUpdated,

        ParliamentaryConstituencyNameFieldUpdated,

        CountryCodeFieldUpdated,

        CountryNameFieldUpdated,

        LondonRegionCodeFieldUpdated,

        LondonRegionNameFieldUpdated
    }
}
