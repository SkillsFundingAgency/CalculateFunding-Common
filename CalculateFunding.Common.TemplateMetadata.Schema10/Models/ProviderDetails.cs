using System;
using System.ComponentModel.DataAnnotations;
using CalculateFunding.Common.TemplateMetadata.Schema10.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// (Optional) details about an provider. Passed through from the provider API.
    /// </summary>
    public class ProviderDetails
    {
        /// <summary>
        /// Date Opened.
        /// </summary>
        [JsonProperty("dateOpened")]
        public DateTimeOffset? DateOpened { get; set; }

        /// <summary>
        /// Date Closed.
        /// </summary>
        [JsonProperty("dateClosed")]
        public DateTimeOffset? DateClosed { get; set; }

        /// <summary>
        /// Status of the organisation
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Phase of Education
        /// </summary>
        [JsonProperty("phaseOfEducation")]
        public string PhaseOfEducation { get; set; }

        /// <summary>
        /// Local Authority Name
        /// </summary>
        [JsonProperty("localAuthorityName")]
        public string LocalAuthorityName { get; set; }

        /// <summary>
        /// Optional open reason from the list of GIAS Open Reasons
        /// </summary>
        [EnumDataType(typeof(ProviderOpenReason))]
        [JsonProperty("openReason")]
        public ProviderOpenReason? OpenReason { get; set; }

        /// <summary>
        /// Optional close reason from list of GIAS Close Reasons
        /// </summary>
        [EnumDataType(typeof(ProviderCloseReason))]
        [JsonProperty("closeReason")]
        public ProviderCloseReason? CloseReason { get; set; }

        /// <summary>
        /// Trust Status
        /// </summary>
        [EnumDataType(typeof(TrustStatus))]
        [JsonProperty("trustStatus")]
        public TrustStatus TrustStatus { get; set; }

        /// <summary>
        /// Trust Name
        /// </summary>
        [JsonProperty("trustName")]
        public string TrustName { get; set; }

        /// <summary>
        /// Town
        /// </summary>
        [JsonProperty("town")]
        public string Town { get; set; }

        /// <summary>
        /// Postcode
        /// </summary>
        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        /// <summary>
        /// Companies House Number
        /// </summary>
        [JsonProperty("companiesHouseNumber")]
        public string CompaniesHouseNumber { get; set; }

        /// <summary>
        /// Group ID
        /// </summary>
        [JsonProperty("groupIdNumber")]
        public string GroupIDNumber { get; set; }

        /// <summary>
        /// RSC Region Name
        /// </summary>
        [JsonProperty("rscRegionName")]
        public string RSCRegionName { get; set; }

        /// <summary>
        /// RSC Region Code
        /// </summary>
        [JsonProperty("rscRegionCode")]
        public string RSCRegionCode { get; set; }

        /// <summary>
        /// Government Office Region Name
        /// </summary>
        [JsonProperty("governmentOfficeRegionName")]
        public string GovernmentOfficeRegionName { get; set; }

        /// <summary>
        /// Government Office Region Code
        /// </summary>
        [JsonProperty("governmentOfficeRegionCode")]
        public string GovernmentOfficeRegionCode { get; set; }

        /// <summary>
        /// District Name
        /// </summary>
        [JsonProperty("districtName")]
        public string DistrictName { get; set; }

        /// <summary>
        /// District Code
        /// </summary>
        [JsonProperty("districtCode")]
        public string DistrictCode { get; set; }

        /// <summary>
        /// Ward Name
        /// </summary>
        [JsonProperty("wardName")]
        public string WardName { get; set; }

        /// <summary>
        /// Ward Code
        /// </summary>
        [JsonProperty("wardCode")]
        public string WardCode { get; set; }

        /// <summary>
        /// Census Ward Name
        /// </summary>
        [JsonProperty("censusWardName")]
        public string CensusWardName { get; set; }

        /// <summary>
        /// Census Ward Code
        /// </summary>
        [JsonProperty("censusWardCode")]
        public string CensusWardCode { get; set; }

        /// <summary>
        /// Middle Super Output Area Name
        /// </summary>
        [JsonProperty("middleSuperOutputAreaName")]
        public string MiddleSuperOutputAreaName { get; set; }

        /// <summary>
        /// Middle Super Output Area Code
        /// </summary>
        [JsonProperty("middleSuperOutputAreaCode")]
        public string MiddleSuperOutputAreaCode { get; set; }

        /// <summary>
        /// Lower Super Output Area Name
        /// </summary>
        [JsonProperty("lowerSuperOutputAreaName")]
        public string LowerSuperOutputAreaName { get; set; }

        /// <summary>
        /// Lower Super Output Area Code
        /// </summary>
        [JsonProperty("lowerSuperOutputAreaCode")]
        public string LowerSuperOutputAreaCode { get; set; }

        /// <summary>
        /// Parliamentary Constituency Name
        /// </summary>
        [JsonProperty("parliamentaryConstituencyName")]
        public string ParliamentaryConstituencyName { get; set; }

        /// <summary>
        /// Parliamentary Constituency Code
        /// </summary>
        [JsonProperty("parliamentaryConstituencyCode")]
        public string ParliamentaryConstituencyCode { get; set; }

        /// <summary>
        /// Country Code
        /// </summary>
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Country Name
        /// </summary>
        [JsonProperty("countryName")]
        public string CountryName { get; set; }

    }
}