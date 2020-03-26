using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Models
{
    public class SchemaJsonCalculation
    {
        public uint TemplateCalculationId { get; set; }
        
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public string AggregationType { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FormulaText { get; set; }
        
        public string ValueType { get; set; }
        
        public string ValueFormat { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SchemaJsonCalculation> Calculations { get; set; }
    }
}