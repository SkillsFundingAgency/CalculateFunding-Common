using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface IGraphRepository
    {
        Task<IEnumerable<Entity<TNode>>> GetCircularDependencies<TNode>(string relationShip, IField field)
            where TNode : class;
        Task<IEnumerable<Entity<TNode>>> GetAllEntities<TNode>(IField field, IEnumerable<string> relationships)
            where TNode : class;
        Task UpsertNodes<T>(IEnumerable<T> nodes, IEnumerable<string> indices = null);
        Task DeleteNode<T>(IField field);
        Task UpsertRelationship<A, B>(string relationShipName, IField left, IField right);

        Task DeleteRelationship<A, B>(string relationShipName, IField left, IField right);
        Task DeleteNodeAndChildNodes<T>(IField field);
    }
}
