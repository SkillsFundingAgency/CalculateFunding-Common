using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedFunding : IIdentifiable
    {
        public string Id => $"funding_{Current.OrganisationIdentiferType}_{Current.OrganisationIdentifer}_{Current.FundingPeriodId}_{Current.FundingStreamId}";

        public PublishedFundingVersion Current { get; set; }

        public string ParitionKey => Id;
    }
}
