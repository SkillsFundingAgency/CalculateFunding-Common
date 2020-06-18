namespace CalculateFunding.Common.Graph.Neo4J
{
    public interface ICypherBuilderFactory
    {
        ICypherBuilder NewCypherBuilder();
    }
}
