using CalculateFunding.Common.ApiClient.External.Models;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.ApiClient.External.UnitTests
{
    public class AtomFeedBuilder<T> : TestEntityBuilder
        where T : class, new()
    {
        private string _id;

        public AtomFeedBuilder<T> WithId(string id)
        {
            _id = id;

            return this;
        }
        
        public AtomFeed<T> Build()
        {
            return new AtomFeed<T>
            {
                Id = _id ?? NewRandomString(),
            };
        }
    }
}