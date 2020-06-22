using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Models
{
    public class SchemaJsonCalculation
    {
        [JsonIgnore]
        public Guid Id = Guid.NewGuid();
        
        public uint TemplateCalculationId { get; set; }
        
        public string Name { get; set; }
        
        [EnumDataType(typeof(FundingCalculationType))]
        public FundingCalculationType Type { get; set; }
        
        [EnumDataType(typeof(AggregationType))]
        public AggregationType AggregationType { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FormulaText { get; set; }
        
        [EnumDataType(typeof(ValueFormatType))]
        public ValueFormatType ValueFormat { get; set; }
        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> AllowedEnumTypeValues { get; set; }
        
        public SchemaJsonGroupRate GroupRate { get; set; }
        
        public SchemaJsonPercentageChangeBetweenAandB PercentageChangeBetweenAandB { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SchemaJsonCalculation> Calculations { get; set; }
    }
}