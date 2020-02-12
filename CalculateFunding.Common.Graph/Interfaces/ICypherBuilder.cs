using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface ICypherBuilder
    {
        ICypherBuilder AddMatch(string query);
        ICypherBuilder AddWhere(string query);
        ICypherBuilder AddMerge(string query);
        ICypherBuilder AddCreate(string query);
        ICypherBuilder AddUnwind(string query);
        ICypherBuilder AddDetachDelete(string query);
        ICypherBuilder AddSet(string query);
    }
}
