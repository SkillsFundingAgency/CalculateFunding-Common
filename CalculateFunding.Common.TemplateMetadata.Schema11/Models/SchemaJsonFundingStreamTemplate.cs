using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Models
{
    public class SchemaJsonFundingStreamTemplate
    {
        public string TemplateStatus { get; set; }
        public DateTime TemplateStatusChangedDate { get; set; }
        public string TemplateVersion { get; set; }
        public SchemaJsonFundingStream FundingStream { get; set; }
        public IEnumerable<SchemaJsonFundingLine> FundingLines { get; set; }
    }
}