using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public interface Calculation
    {
        string TemplateCalculationId { get; set; }

        string Name { get; set; }

        IEnumerable<ReferenceData> ReferenceData { get; set; }
    }
}