using CalculateFunding.Common.TemplateMetadata.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public class PercentageChangeBetweenAandB
    {
        [JsonProperty("calculationA")]
        public uint CalculationA { get; set; }
            
        [JsonProperty("calculationB")]
        public uint CalculationB { get; set; }
            
        [JsonProperty("calculationAggregationType")]
        public AggregationType CalculationAggregationType { get; set; }   
    }
}