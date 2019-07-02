using CalculateFunding.Common.Models;
using System;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class Period : Reference
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public int StartYear
        {
            get
            {
                return StartDate.Year;
            }
        }

        public int EndYear
        {
            get
            {
                return EndDate.Year;
            }
        }
    }
}
