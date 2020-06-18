using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.Graph.UnitTests.Cosmos
{
    internal class ModelBuilder : TestEntityBuilder
    {
        private string _anIdProperty;
        private string _name;

        public ModelBuilder WithAnIdProperty(string anIdProperty)
        {
            _anIdProperty = anIdProperty;

            return this;
        }

        public ModelBuilder WithName(string name)
        {
            _name = name;

            return this;
        }
            
        public Model Build()
        {
            return new Model
            {
                AnIdProperty =  _anIdProperty ?? NewRandomString(),
                Name = _name ?? NewRandomString()
            };
        }
    }
}