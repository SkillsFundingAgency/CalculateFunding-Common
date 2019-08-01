using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class UpdateCalculationResult
    {
        public Calculation Calculation { get; set; }

        public BuildProject BuildProject { get; set; }

        public CalculationCurrentVersion CurrentVersion { get; set; }
    }
}
