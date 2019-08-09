using System.ComponentModel.DataAnnotations;
using CalculateFunding.Common.ApiClient.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedFundingVersion : VersionedItem
    {
        [JsonProperty("id")]
        public override string Id => $"funding_{OrganisationIdentiferType}_{OrganisationIdentifer}_{FundingPeriodId}_{FundingStreamId}_{Version}";

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("organisationIdentifer")]
        public string OrganisationIdentifer { get; set; }

        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("organisationIdentiferType")]
        [EnumDataType(typeof(OrganisationIdentifierType))]
        public OrganisationIdentifierType OrganisationIdentiferType { get; set; }

        [JsonProperty("entityId")]
        public override string EntityId => $"funding_{OrganisationIdentiferType}_{OrganisationIdentifer}_{FundingPeriodId}_{FundingStreamId}";

        public override VersionedItem Clone()
        {
            // Serialise to perform a deep copy
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<PublishedFundingVersion>(json);
        }
    }
}
