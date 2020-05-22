using CalculateFunding.Common.ApiClient.External.Models;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.ApiClient.External.UnitTests
{
    public class FundingPeriodBuilder : TestEntityBuilder
    {
        private string _id;
        private string _name;

        public FundingPeriodBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public FundingPeriodBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public FundingPeriod Build()
        {
            return new FundingPeriod
            {
                Id = _id ?? NewRandomString(),
                Name = _name ?? NewRandomString()
            };
        }
    }
}
