using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies.Models.ViewModels
{
    public class FundingConfigurationUpdateViewModel
    {
        public IEnumerable<OrganisationGroupingConfiguration> OrganisationGroupings { get; set; }

        public string DefaultTemplateVersion { get; set; }
        
        public ApprovalMode ApprovalMode { get; set; }
        
    }
}
