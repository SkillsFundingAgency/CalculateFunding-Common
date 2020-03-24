using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface IGraphRepository
    {
        Task<IEnumerable<Entity<TNode, TRelationship>>> GetCircularDependencies<TNode, TRelationship>(string relationShip, IField field)
            where TNode : class
            where TRelationship : class;
        Task UpsertNodes<T>(IEnumerable<T> nodes, IEnumerable<string> indices = null);
        Task DeleteNode<T>(IField field);
        Task UpsertRelationship<A, B>(string relationShipName, IField left, IField right);

        Task DeleteRelationship<A, B>(string relationShipName, IField left, IField right);
        Task DeleteNodeAndChildNodes<T>(IField field);
    }
}
