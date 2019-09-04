using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.ApiClient.Specifications.UnitTests
{
    public class SummarySpecificationBuilder
    {
        public SpecificationSummary Build()
        {
            return new SpecificationSummary
            {
                Id = new RandomString()
            };
        }
    }
}