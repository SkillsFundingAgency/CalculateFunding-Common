namespace CalculateFunding.Common.Graph.Neo4J
{
    public interface ICypherBuilder
    {
        ICypherBuilder AddDetachDelete(string query);
        ICypherBuilder AddUnwind(string query);
        ICypherBuilder AddMatch(IMatch[] matches);
        ICypherBuilder AddMerge(string query);
        ICypherBuilder AddWhere(string query);
        ICypherBuilder AddAnd(string query);
        ICypherBuilder AddCreate(string query);
        ICypherBuilder AddSet(string query);
        ICypherBuilder AddDelete(string query);
        ICypherBuilder AddReturn(string[] returns);
    }
}
