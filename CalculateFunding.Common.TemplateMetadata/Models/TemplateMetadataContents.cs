using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public class TemplateMetadataContents
    {
        public IEnumerable<FundingLine> RootFundingLines { get; set; }
        public string SchemaVersion { get; set; }
    }
}
