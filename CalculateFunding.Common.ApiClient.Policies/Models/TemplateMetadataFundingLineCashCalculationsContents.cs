using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class TemplateMetadataFundingLineCashCalculationsContents
    {
        public string FundingStreamId { get; set; }
        public string FundingPeriodId { get; set; }
        public string TemplateVersion { get; set; }
        public IEnumerable<TemplateMetadataFundingLine> FundingLines { get; set; }
        public IDictionary<string, IEnumerable<TemplateMetadataCalculation>> CashCalculations { get; set; }
        public string SchemaVersion { get; set; }
    }
}
