using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.Graph.UnitTests.Cosmos
{
    public class AmendRelationshipRequestBuilder : TestEntityBuilder
    {
        private Field _a;
        private Field _b;
        private string _type;

        public AmendRelationshipRequestBuilder WithA(Field a)
        {
            _a = a;

            return this;
        }

        public AmendRelationshipRequestBuilder WithB(Field b)
        {
            _b = b;

            return this;
        }

        public AmendRelationshipRequestBuilder WithType(string type)
        {
            _type = type;

            return this;
        }
        
        public AmendRelationshipRequest Build()
        {
            return new AmendRelationshipRequest
            {
                A = _a,
                B = _b,
                Type = _type
            };
        }
    }
}