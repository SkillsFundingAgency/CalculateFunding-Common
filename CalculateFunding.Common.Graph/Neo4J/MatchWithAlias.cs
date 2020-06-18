namespace CalculateFunding.Common.Graph.Neo4J
{
    public class MatchWithAlias : IMatch
    {
        public string Pattern { get; set; }
        public string Alias { get; set; }
    }
}
