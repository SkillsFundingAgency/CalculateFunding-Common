namespace CalculateFunding.Common.ApiClient.FDZ.Models
{
    public class PaymentOrganisation
    {
        /// <summary>
        /// ID of this organisation within the publishing area
        /// </summary>
        public int PaymentOrganisationId { get; set; }

        /// <summary>
        /// The provider snapshot this organisation is part of
        /// </summary>
        public int ProviderSnapshotId { get; set; }

        /// <summary>
        /// Organisation name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of organisation, eg Local Authority or Academy Trust
        /// </summary>
        public string OrganisationType { get; set; }

        public string Ukprn { get; set; }

        public string Upin { get; set; }

        public string TrustCode { get; set; }

        public string Urn { get; set; }

        public string LaCode { get; set; }

        public string CompanyHouseNumber { get; set; }
    }
}
