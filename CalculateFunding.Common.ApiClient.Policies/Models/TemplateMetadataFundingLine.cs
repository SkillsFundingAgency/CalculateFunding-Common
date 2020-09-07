using CalculateFunding.Common.TemplateMetadata.Enums;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class TemplateMetadataFundingLine
    {
        public string Name { get; set; }
        public string FundingLineCode { get; set; }
        public uint TemplateLineId { get; set; }
        public FundingLineType Type { get; set; }
    }
}
