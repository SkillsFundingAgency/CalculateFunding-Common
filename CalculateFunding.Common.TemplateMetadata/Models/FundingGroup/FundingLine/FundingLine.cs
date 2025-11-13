using System.Collections.Generic;
using CalculateFunding.Common.TemplateMetadata.Enums;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public class FundingLine
    {
        public FundingLine()
        {
        }

        public string Name { get; set; }

        public string ExternalFundingLineName { get; set; }

        public string FundingLineCode { get; set; }
        
        public decimal Value { get; set; }

        public uint TemplateLineId { get; set; }

        public FundingLineType Type { get; set; }

        public IEnumerable<DistributionPeriod> DistributionPeriods { get; set; }

        public IEnumerable<Calculation> Calculations { get; set; }

        public IEnumerable<FundingLine> FundingLines { get; set; }
    }
}