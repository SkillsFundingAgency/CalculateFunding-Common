using System.Collections.Generic;
using CalculateFunding.Common.TemplateMetadata.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    /// <summary>
    /// A calculation used to build up a funding line.
    /// </summary>
    public class Calculation
    {
        public Calculation()
        {
            AggregationType = AggregationType.Sum;
        }

        public string Name { get; set; }

        public uint TemplateCalculationId { get; set; }

        public object Value { get; set; }

        public CalculationValueFormat ValueFormat { get; set; }
        
        public IEnumerable<string> AllowedEnumTypeValues { get; set; }

        public CalculationType Type { get; set; }

        public string FormulaText { get; set; }

        public AggregationType AggregationType { get; set; }
        
        public GroupRate GroupRate { get; set; }

        public PercentageChangeBetweenAandB PercentageChangeBetweenAandB { get; set; }

        public IEnumerable<Calculation> Calculations { get; set; }

        public IEnumerable<ReferenceData> ReferenceData { get; set; }
    }
}