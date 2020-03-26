using System.Collections.Concurrent;
using System.Collections.Generic;
using CalculateFunding.Common.TemplateMetadata.Models;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Validators
{
    internal class TemplateMetadataValidatorContext
    {
        internal TemplateMetadataValidatorContext()
        {
            CalculationDictionary = new ConcurrentDictionary<uint, Calculation>();
            FundingLineDictionary = new ConcurrentDictionary<uint, FundingLine>();
            CalculationTemplateCalcIds = new ConcurrentDictionary<string, ICollection<uint>>();
            FundingLineTemplateIds = new ConcurrentDictionary<string, ICollection<uint>>();
        }

        internal ConcurrentDictionary<uint, Calculation> CalculationDictionary { get; }

        internal ConcurrentDictionary<uint, FundingLine> FundingLineDictionary { get; }

        internal ConcurrentDictionary<string, ICollection<uint>> CalculationTemplateCalcIds { get; }

        internal ConcurrentDictionary<string, ICollection<uint>> FundingLineTemplateIds { get; }
    }
}