namespace CalculateFunding.Common.CosmosDb
{
    public class CosmosDbQueryParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public CosmosDbQueryParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
