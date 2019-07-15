using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.UnitTests.Utilities.Builders;

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