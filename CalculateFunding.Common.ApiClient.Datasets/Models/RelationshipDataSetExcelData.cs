using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class RelationshipDataSetExcelData
    {
        public RelationshipDataSetExcelData(string ukprn)
        {
            Ukprn = ukprn;
            FundingLines = new Dictionary<string, decimal?>();
            Calculations = new Dictionary<string, object>();
        }

        public string Ukprn { get; }

        public IDictionary<string, decimal?> FundingLines { get; set; }

        public IDictionary<string, object> Calculations { get; set; }
    }
}
