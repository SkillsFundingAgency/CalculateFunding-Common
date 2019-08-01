using System;
using System.Collections.Generic;
using System.Text;
using CalculateFunding.Common.Models.Aggregations;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationStatusCounts : StatusCounts
    {
        public string SpecificationId { get; set; }
    }
}
