﻿using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Providers.Models.Search
{
    public class ProviderVersionSearchResult
    {
        public string Id { get; set; }

        public string ProviderVersionId { get; set; }

        public string ProviderId { get; set; }

        public string Name { get; set; }

        public string URN { get; set; }

        public string UKPRN { get; set; }

        public string UPIN { get; set; }

        public string EstablishmentNumber { get; set; }

        public string DfeEstablishmentNumber { get; set; }

        public string Authority { get; set; }

        public string ProviderType { get; set; }

        public string ProviderSubType { get; set; }

        public DateTimeOffset? DateOpened { get; set; }

        public DateTimeOffset? DateClosed { get; set; }

        public string ProviderProfileIdType { get; set; }

        public string LaCode { get; set; }
        public string LaOrg { get; set; }

        public string NavVendorNo { get; set; }

        public string CrmAccountId { get; set; }

        public string LegalName { get; set; }

        public string Status { get; set; }

        public string PhaseOfEducation { get; set; }

        public string ReasonEstablishmentOpened { get; set; }

        public string ReasonEstablishmentClosed { get; set; }

        public string Successor { get; set; }

        public string TrustStatus { get; set; }

        public string TrustName { get; set; }

        public string TrustCode { get; set; }

        public string Town { get; set; }

        public string Postcode { get; set; }

        public string RscRegionName { get; set; }

        public string RscRegionCode { get; set; }

        public string LocalGovernmentGroupTypeName { get; set; }

        public string LocalGovernmentGroupTypeCode { get; set; }

        public string GovernmentOfficeRegionName { get; set; }

        public string GovernmentOfficeRegionCode { get; set; }

        public string CountryName { get; set; }

        public string CountryCode { get; set; }

        public string LondonRegionName { get; set; }

        public string LondonRegionCode { get; set; }

        public string Street { get; set; }

        public string Locality { get; set; }

        public string Address3 { get; set; }

        public string PaymentOrganisationIdentifier { get; set; }

        public string PaymentOrganisationName { get; set; }

        public string ProviderTypeCode { get; set; }

        public string ProviderSubTypeCode { get; set; }

        public string PreviousLACode { get; set; }

        public string PreviousLAName { get; set; }

        public string PreviousEstablishmentNumber { get; set; }

        public string FurtherEducationTypeCode { get; set; }

        public string FurtherEducationTypeName { get; set; }

        public IEnumerable<string> Predecessors { get; set; }

        public IEnumerable<string> Successors { get; set; }

        public string PhaseOfEducationCode { get; set; }

        public string StatutoryLowAge { get; set; }

        public string StatutoryHighAge { get; set; }

        public string OfficialSixthFormCode { get; set; }

        public string OfficialSixthFormName { get; set; }

        public string PreviousLaCode { get; set; }

        public string PreviousLaName { get; set; }

        public string StatusCode { get; set; }

        public string ReasonEstablishmentOpenedCode { get; set; }

        public string ReasonEstablishmentClosedCode { get; set; }
    }
}
