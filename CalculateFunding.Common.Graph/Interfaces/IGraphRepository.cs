using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface IGraphRepository
    {
        Task UpsertNodes<T>(IEnumerable<T> nodes, IEnumerable<string> indices = null);
        Task DeleteNode<T>(string field, string value);
        Task UpsertRelationship<A, B>(string relationShipName, (string field, string value) left, (string field, string value) right);

        Task DeleteRelationship<A, B>(string relationShipName, (string field, string value) left, (string field, string value) right);
        Task DeleteNodeAndChildNodes<T>(string field, string value);
    }
}
