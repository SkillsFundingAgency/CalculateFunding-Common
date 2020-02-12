﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface IGraphRepository
    {
        Task AddNodes<T>(IList<T> nodes, IEnumerable<string> indices = null);
        Task DeleteNode<T>(string field, string value);
        Task CreateRelationship<A, B>(string relationShipName, (string field, string value) left, (string field, string value) right);
    }
}