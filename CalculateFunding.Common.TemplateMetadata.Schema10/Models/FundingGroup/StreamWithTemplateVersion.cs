using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    /// <summary>
    /// Extension of a stream that also holds informaton about which version of the template is used.
    /// </summary>
    public class StreamWithTemplateVersion : Stream
    {
        /// <summary>
        /// The version of the template (e.g. this is Version 2 of PE and sport template).
        /// </summary>
        [JsonProperty("templateVersion")]
        public string TemplateVersion { get; set; }
    }
}