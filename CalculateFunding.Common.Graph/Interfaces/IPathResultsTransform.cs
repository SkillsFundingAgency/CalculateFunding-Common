using System.Collections.Generic;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface IPathResultsTransform
    {
        IEnumerable<Entity<TNode>> TransformMatches<TNode>(IEnumerable<Dictionary<string, object>> pathResults,
            string vertexLabel,
            IField identifier)
            where TNode : class;
        
        IEnumerable<Entity<TNode>> TransformAll<TNode>(IEnumerable<Dictionary<string, object>> pathResults,
            string vertexLabel,
            IField identifier)
            where TNode : class;
    }
}