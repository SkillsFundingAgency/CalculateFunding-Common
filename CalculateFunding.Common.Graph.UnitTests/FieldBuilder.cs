using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.Graph.UnitTests
{
    public class FieldBuilder : TestEntityBuilder
    {
        private string _name;
        private string _value;

        public FieldBuilder WithName(string name)
        {
            _name = name;

            return this;
        }

        public FieldBuilder WithValue(string value)
        {
            _value = value;

            return this;
        }
        
        public Field Build()
        {
            return new Field
            {
                Name = _name ?? NewRandomString(),
                Value = _value ?? NewRandomString()
            };
        }
    }
}