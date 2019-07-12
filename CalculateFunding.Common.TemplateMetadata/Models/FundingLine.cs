using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public interface FundingLine
    {
        string ReferenceId { get; set; }

        string Name { get; set; }

        IEnumerable<FundingLine> FundingLines { get; set; }

        IEnumerable<Calculation> Calculations { get; set; }
    }
}