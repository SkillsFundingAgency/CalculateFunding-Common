using System.Collections.Generic;
using CalculateFunding.Common.TemplateMetadata.Enums;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public class Calculation
    {
        public uint TemplateCalculationId { get; set; }

        public string Name { get; set; }

        public CalculationValueFormat ValueFormat { get; set; }

        public CalculationType Type { get; set; }

        public AggregationType AggregationType { get; set; }

        public IEnumerable<ReferenceData> ReferenceData { get; set; }
    }
}