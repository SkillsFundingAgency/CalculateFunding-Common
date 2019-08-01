using CalculateFunding.Common.Models.Aggregations;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationStatusCountsModel : StatusCounts
    {
        public string SpecificationId { get; set; }
    }
}
