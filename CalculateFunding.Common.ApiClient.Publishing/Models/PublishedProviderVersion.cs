using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.ApiClient.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    /// <summary>
    /// A version of PublishedProvider. Used to track these properties over time each time an update is made from refresh, approve or publish funding.
    /// </summary>
    public class PublishedProviderVersion : VersionedItem
    {
        /// <summary>
        /// Cosmos ID for the document. This will be used as the document ID when saving to cosmos
        /// </summary>
        [JsonProperty("id")]
        public override string Id => $"publishedprovider-{FundingStreamId}-{FundingPeriodId}-{ProviderId}-{Version}";
        
        /// <summary>
        /// Logical ID for this published provider to identify it between datastores and consistent between versions
        /// </summary>
        [JsonProperty("publishedProviderId")]
        public string PublishedProviderId => $"{FundingStreamId}-{FundingPeriodId}-{ProviderId}";

        /// <summary>
        /// Funding Stream ID. eg PSG, DSG
        /// </summary>
        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        /// <summary>
        /// Funding Period ID - Will be in the format of Funding Period Type Id-Funding Period eg AY-1920 or FY-2021
        /// </summary>
        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        /// <summary>
        /// Specification this ID is associated with
        /// </summary>
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        /// <summary>
        /// Entity ID for cosmos versioning. This refers to the parent PublishedProvider cosmos ID
        /// </summary>
        [JsonProperty("entityId")]
        public override string EntityId => $"publishedprovider-{FundingStreamId}-{FundingPeriodId}-{ProviderId}-{Version}";

        /// <summary>
        /// Published Provider Approval Status
        /// </summary>
        [JsonProperty("status")]
        public PublishedProviderStatus Status { get; set; }

        /// <summary>
        /// Provider ID (UKPRN) - eg 10001002
        /// </summary>
        [JsonProperty("providerId")]
        public string ProviderId { get; set; }

        /// <summary>
        /// Partition key for cosmos - used the the publishedprovider collection. This document should be kept in the same paritition as the parent PublishedProvider, so will match the parent PublishedProvider comos ID.
        /// </summary>
        [JsonProperty("partitionKey")]
        public string PartitionKey => $"publishedprovider-{FundingStreamId}-{FundingPeriodId}-{ProviderId}";

        /// <summary>
        /// Funding Lines - used to store the profiling result and total for all funding lines.
        /// The total funding per funding line and distribution periods are stored here.
        /// This will be consumed from the organisation group aggregator and variations over time.
        /// </summary>
        [JsonProperty("fundingLines")]
        public IEnumerable<FundingLine> FundingLines { get; set; }

        /// <summary>
        /// Calculations - used to store all calculations.
        /// </summary>
        [JsonProperty("calculations")]
        public IEnumerable<FundingCalculation> Calculations { get; set; }

        /// <summary>
        /// Payment Funding Lines - used to store the profiling result and total for all payment funding lines.
        /// The total funding per funding line and distribution periods are stored here.
        /// </summary>
        [JsonProperty("paymentFundingLines")]
        public IDictionary<string, IEnumerable<FundingLine>> PaymentFundingLines { get; set; }

        /// <summary>
        /// Total funding for this provider
        /// </summary>
        [JsonProperty("totalFunding")]
        public decimal? TotalFunding { get; set; }

        /// <summary>
        /// Major version
        /// </summary>
        [JsonProperty("majorVersion")]
        public int MajorVersion { get; set; }

        /// <summary>
        /// Minor version
        /// </summary>
        [JsonProperty("minorVersion")]
        public int MinorVersion { get; set; }

        /// <summary>
        /// Provider information
        /// </summary>
        [JsonProperty("provider")]
        public Provider Provider { get; set; }

        /// <summary>
        /// Provider IDs of Predecessor providers
        /// </summary>
        [JsonProperty("predecessors")]
        public IEnumerable<string> Predecessors { get; set; }

        /// <summary>
        /// Variation reasons
        /// </summary>
        [JsonProperty("variationReasons")]
        public IEnumerable<VariationReason> VariationReasons { get; set; }
        
        /// <summary>
        /// Errors blocking the release of this funding encountered
        /// during the publishing cycle (approved, refresh, publish) that created this version
        /// </summary>
        [JsonProperty("errors")]
        public ICollection<PublishedProviderError> Errors { get; set; }
        
        /// <summary>
        /// The none default profiling patterns used for this provider
        /// in this period and funding stream keyed by funding line
        /// </summary>
        [JsonProperty("profilePatternKeys")]
        public ICollection<ProfilePatternKey> ProfilePatternKeys { get; set; }
        
        /// <summary>
        /// The custom profile periods used for this provider
        /// in this period and funding stream keyed by funding line
        /// </summary>
        [JsonProperty("customProfiles")]
        public ICollection<FundingLineProfileOverrides> CustomProfiles { get; set; }
        
        /// <summary>
        /// Flag indicating whether this provider has any custom profiles 
        /// </summary>
        [JsonProperty("hasCustomProfiles")]
        public bool HasCustomProfiles => CustomProfiles?.Any() == true;

        /// <summary>
        /// Job ID this PublishedProvider was updated or created on
        /// </summary>
        [JsonProperty("jobId")]
        public string JobId { get; set; }

        /// <summary>
        /// Correlation ID this PublishedProvider was updated or created on.
        /// This should line up with Provider variations and all updated made in a single refresh/approve/publish should have the same id
        /// </summary>
        [JsonProperty("correlationId")]
        public string CorrelationId { get; set; }
        
        /// <summary>
        /// Collection of carry over payments keyed by funding line for the funding period
        /// this published provider version is in
        /// </summary>
        [JsonProperty("carryOvers")]
        public ICollection<ProfilingCarryOver> CarryOvers { get; set; }
        
        /// <summary>
        /// Indicates whether the allocations for this version
        /// are indicative of payment if the provider converts
        /// </summary>
        [JsonProperty("isIndicative")]
        public bool IsIndicative { get; set; }

        /// <summary>
        /// Collection of profiling audits for each funding line profile updates
        /// </summary>
        [JsonProperty("profilingAudits")]
        public ICollection<ProfilingAudit> ProfilingAudits { get; set; }

        public override VersionedItem Clone()
        {
            // Serialise to perform a deep copy
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<PublishedProviderVersion>(json);
        }
    }
}
