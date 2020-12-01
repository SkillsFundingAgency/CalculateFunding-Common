using CalculateFunding.Common.Models;
using CalculateFunding.Common.Models.Versioning;

namespace CalculateFunding.Common.ApiClient.CalcEngine.Models
{
    public class CalculationSummaryModel : Reference
    {
        public CalculationType CalculationType { get; set; }

        public PublishStatus Status { get; set; }

        public int Version { get; set; }

        public CalculationValueType CalculationValueType { get; set; }
    }
}
