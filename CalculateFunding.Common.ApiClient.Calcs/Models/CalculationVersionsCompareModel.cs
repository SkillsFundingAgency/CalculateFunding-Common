using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationVersionsCompareModel
    {
        public string CalculationId { get; set; }

        public IEnumerable<int> Versions { get; set; }
    }
}
