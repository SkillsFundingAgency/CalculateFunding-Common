using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    /// <summary>
    /// Valid list of organisation group types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrganisationGroupTypeCode
    {
        /// <summary>
        /// Local Authority (e.g. Warwickshire).
        /// </summary>
        LocalAuthority,

        /// <summary>
        /// Academy Trust (e.g. Star Foundation).
        /// </summary>
        AcademyTrust,

        /// <summary>
        /// Regional Schools Commissioner Region (e.g. Lancashire and West Yorkshire).
        /// </summary>
        RSCRegion,

        /// <summary>
        /// Government Office Region, (e.g. North West).
        /// </summary>
        GovernmentOfficeRegion,

        /// <summary>
        /// District (e.g. Hyndburn).
        /// </summary>
        District,

        /// <summary>
        /// Ward (e.g. Milnshaw).
        /// </summary>
        Ward,

        /// <summary>
        /// Census Ward.
        /// </summary>
        CensusWard,

        /// <summary>
        /// Middle Super Output Area (e.g. Mansfield 002).
        /// </summary>
        MiddleSuperOutputArea,

        /// <summary>
        /// Lower Super Output Area (e.g. Mansfield 002A).
        /// </summary>
        LowerSuperOutputArea,

        /// <summary>
        /// Parlimentry constituency (e.g. Mansfield).
        /// </summary>
        ParliamentaryConstituency,

        /// <summary>
        /// Provider
        /// </summary>
        Provider,

        /// <summary>
        /// Region
        /// </summary>
        Region,

        /// <summary>
        /// Country eg GB
        /// </summary>
        Country,

        /// <summary>
        /// Local Government Group
        /// </summary>
        LocalGovernmentGroup,

        /// <summary>
        /// Local Authority Ssf
        /// </summary>
        LocalAuthoritySsf,

        /// <summary>
        /// Local Authority Mss
        /// </summary>
        LocalAuthorityMss,

        /// <summary>
        /// Local Authority Maintained
        /// </summary>
        LocalAuthorityMaintained,

        /// <summary>
        /// Mainstream
        /// </summary>
        Mainstream,

        /// <summary>
        /// Mainstream Academy
        /// </summary>
        MainstreamAcademy,

        /// <summary>
        /// Non-maintained Special
        /// </summary>
        NonMaintainedSpecial,

        /// <summary>
        /// Pupil Referral Unit
        /// </summary>
        PupilReferralUnit,

        /// <summary>
        /// Alternative provision Academy
        /// </summary>
        APAcademy,

        /// <summary>
        /// Academy Special
        /// </summary>
        AcademySpecial,

        /// <summary>
        /// Special
        /// </summary>
        Special
    }
}