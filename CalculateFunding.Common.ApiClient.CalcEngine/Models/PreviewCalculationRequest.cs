using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.CalcEngine.Models
{
    public class PreviewCalculationRequest
    {
        public byte[] AssemblyContent { get; set; }
        public CalculationSummaryModel PreviewCalculationSummaryModel { get; set; }

        public IEnumerable<CalculationAggregationData> CalculationAggregationData { get; set; }
    }
}
