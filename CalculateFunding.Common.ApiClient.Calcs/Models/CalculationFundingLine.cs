using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationFundingLine
    {
        [JsonProperty("templateId")]
        public uint TemplateId { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}