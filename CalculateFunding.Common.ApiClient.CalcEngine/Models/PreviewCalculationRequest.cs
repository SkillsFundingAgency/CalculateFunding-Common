namespace CalculateFunding.Common.ApiClient.CalcEngine.Models
{
    public class PreviewCalculationRequest
    {
        public byte[] AssemblyContent { get; set; }
        public CalculationSummaryModel PreviewCalculationSummaryModel { get; set; }
    }
}
