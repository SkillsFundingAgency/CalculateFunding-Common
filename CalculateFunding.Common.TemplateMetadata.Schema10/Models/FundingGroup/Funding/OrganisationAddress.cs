using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OrganisationAddress
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("town")]
        public string Town { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("postcode")]
        public string Postcode { get; set; }
    }
}