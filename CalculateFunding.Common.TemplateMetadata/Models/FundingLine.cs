using System.Collections.Generic;
using CalculateFunding.Common.TemplateMetadata.Enums;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public class FundingLine
    {
        public uint ReferenceId { get; set; }

        public string Name { get; set; }

        public string FundingLineCode { get; set; }

        public FundingLineType Type { get; set; }

        public IEnumerable<FundingLine> FundingLines { get; set; }

        public IEnumerable<Calculation> Calculations { get; set; }
    }
}