using CalculateFunding.Common.ApiClient.Policies.Models;

namespace CalculateFunding.Generators.OrganisationGroup.Extensions
{
    public static class GroupingReasonExtensionMethods
    {
        public static bool IsForProviderPayment(this GroupingReason groupingReason)
        {
            return groupingReason == GroupingReason.Contracting || groupingReason == GroupingReason.Payment;
        }
    }
}
