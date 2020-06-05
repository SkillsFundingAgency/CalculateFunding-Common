using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public class GroupRate
    {
        [JsonProperty("numerator")]
        public uint Numerator { get; set; }
        
        [JsonProperty("denominator")]
        public uint Denominator { get; set; }
    }
}