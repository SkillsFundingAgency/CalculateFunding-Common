namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class PreviewRequest
    {
        public string SpecificationId { get; set; }
        public string CalculationId { get; set; }
        public string SourceCode { get; set; }
        public string Name { get; set; }
        public string ProviderId { get; set; }
        public CalculationDataType DataType { get; set; }
    }
}

