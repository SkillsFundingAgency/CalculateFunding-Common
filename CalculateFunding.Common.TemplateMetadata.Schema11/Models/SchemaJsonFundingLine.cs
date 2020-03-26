using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Models
{
    public class SchemaJsonFundingLine
    {
        public string Name { get; set; }
        public string FundingLineCode { get; set; }
        public uint TemplateLineId { get; set; }
        public string Type { get; set; }
        public string AggregationType { get; set; }
        public string ValueType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SchemaJsonFundingLine> FundingLines { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<SchemaJsonCalculation> Calculations { get; set; }
    }
}