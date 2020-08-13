using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies.Models.ViewModels
{
    public class FundingDateUpdateViewModel
    {
        public IEnumerable<FundingDatePattern> Patterns { get; set; }
    }
}
