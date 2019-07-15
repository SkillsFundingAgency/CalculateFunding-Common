using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// Details around a funding stream.
    /// </summary>
    public class Stream
    {
        /// <summary>
        /// The code for the funding stream (e.g. PESport).
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// The name of the funding stream (e.g. PE Sport &amp; Premium).
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}