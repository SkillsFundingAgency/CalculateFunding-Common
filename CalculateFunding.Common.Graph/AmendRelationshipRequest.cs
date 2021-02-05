namespace CalculateFunding.Common.Graph
{
    public class AmendRelationshipRequest
    {
        public string Type { get; set; }
        
        public Field A { get; set; }
        
        public Field B { get; set; }
    }
}