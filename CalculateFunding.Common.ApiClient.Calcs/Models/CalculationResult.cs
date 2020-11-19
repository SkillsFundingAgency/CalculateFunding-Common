using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationResult
    {
        public Reference Calculation { get; set; }

        public object Value { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionStackTrace { get; set; }

        public CalculationType CalculationType { get; set; }

        public CalculationDataType CalculationDataType { get; set; }
    }
}
