using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public class TemplateMetadataContents
    {
        public string FundingStreamId { get; set; }
        
        public string FundingPeriodId { get; set; }
        
        public string TemplateVersion { get; set; }
        
        public IEnumerable<FundingLine> RootFundingLines { get; set; }
        public string SchemaVersion { get; set; }
        
        public DateTimeOffset LastModified { get; set; }
    }
}
