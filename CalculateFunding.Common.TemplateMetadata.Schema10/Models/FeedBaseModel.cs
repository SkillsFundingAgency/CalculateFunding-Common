using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// Base model for the feed method.
    /// </summary>
    public class FeedBaseModel
    {
        /// <summary>
        /// Schema URI to validate against.
        /// </summary>
        [JsonProperty("$schema")]
        public string SchemaUri { get; set; }

        /// <summary>
        /// The schema version. Schema here refers to the Classes etc.., not the specific model being used.
        /// </summary>
        [JsonProperty("schemaVersion")]
        public string SchemaVersion { get; set; }

        /// <summary>
        /// The funding group (a parent grouping organisation - such as an LA, MAT, Region etc...).
        /// </summary>
        [JsonProperty("funding")]
        public FundingFeed Funding { get; set; }
    }
}