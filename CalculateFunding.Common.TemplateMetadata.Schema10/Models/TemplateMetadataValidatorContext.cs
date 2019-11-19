using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Models
{
    internal class TemplateMetadataValidatorContext
    {
        internal TemplateMetadataValidatorContext()
        {
            CalculationDictionary = new ConcurrentDictionary<uint, Calculation>();
            FundingLineDictionary = new ConcurrentDictionary<uint, FundingLine>();
            ReferenceDataDictionary = new ConcurrentDictionary<uint, ReferenceData>();
            CalculationTemplateCalcIds = new ConcurrentDictionary<string, ICollection<uint>>();
            FundingLineTemplateIds = new ConcurrentDictionary<string, ICollection<uint>>();
        }

        internal ConcurrentDictionary<uint, Calculation> CalculationDictionary { get; }

        internal ConcurrentDictionary<uint, FundingLine> FundingLineDictionary { get; }

        internal ConcurrentDictionary<uint, ReferenceData> ReferenceDataDictionary { get; }

        internal ConcurrentDictionary<string, ICollection<uint>> CalculationTemplateCalcIds { get; }

        internal ConcurrentDictionary<string, ICollection<uint>> FundingLineTemplateIds { get; }
    }
}
