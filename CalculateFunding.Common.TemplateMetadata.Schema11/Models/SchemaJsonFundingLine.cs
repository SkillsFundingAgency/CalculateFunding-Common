using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Models
{
    public class SchemaJsonFundingLine
    {
        public uint TemplateLineId { get; set; }
        
        [EnumDataType(typeof(FundingLineType))]
        public FundingLineType Type { get; set; }
        
        public string Name { get; set; }
        
        public string FundingLineCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SchemaJsonFundingLine> FundingLines { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SchemaJsonCalculation> Calculations { get; set; }
    }
}