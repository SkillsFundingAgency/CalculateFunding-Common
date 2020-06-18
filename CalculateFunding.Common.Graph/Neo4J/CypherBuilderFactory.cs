namespace CalculateFunding.Common.Graph.Neo4J
{
    public class CypherBuilderFactory : ICypherBuilderFactory
    {
        public ICypherBuilder NewCypherBuilder()
        {
            return new CypherBuilder();
        }
    }
}
