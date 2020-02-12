using CalculateFunding.Common.Graph.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Graph
{
    public class CypherBuilder : ICypherBuilder
    {
        private StringBuilder _stringBuilder;

        public CypherBuilder()
        {
            _stringBuilder = new StringBuilder();
        }

        public ICypherBuilder AddDetachDelete(string query)
        {
            _stringBuilder.AppendLine($"DETACH DELETE {query}");
            return this;
        }

        public ICypherBuilder AddDelete(string query)
        {
            _stringBuilder.AppendLine($"DELETE {query}");
            return this;
        }
        public ICypherBuilder AddUnwind(string query)
        {
            _stringBuilder.AppendLine($"UNWIND {query}");
            return this;
        }

        public ICypherBuilder AddMatch(string query)
        {
            _stringBuilder.AppendLine($"MATCH({query})");
            return this;
        }

        public ICypherBuilder AddMerge(string query)
        {
            _stringBuilder.AppendLine($"MERGE({query})");
            return this;
        }

        public ICypherBuilder AddWhere(string query)
        {
            _stringBuilder.AppendLine($"WHERE {query}");
            return this;
        }

        public ICypherBuilder AddCreate(string query)
        {
            _stringBuilder.AppendLine($"CREATE {query}");
            return this;
        }

        public ICypherBuilder AddSet(string query)
        {
            _stringBuilder.AppendLine($"SET {query}");
            return this;
        }

        public override string ToString()
        {
            try
            {
                return _stringBuilder.ToString();
            }
            finally
            {
                _stringBuilder.Clear();
            }
        }
    }
}
