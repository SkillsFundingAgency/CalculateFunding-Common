using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Schema12.Models
{
    public class SchemaJsonFundingStreamTemplate
    {
        public IEnumerable<SchemaJsonFundingLine> FundingLines { get; set; }
    }
}