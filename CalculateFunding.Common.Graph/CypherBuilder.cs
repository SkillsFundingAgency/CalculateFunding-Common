using System.Text;

namespace CalculateFunding.Common.Graph
{
    public class CypherBuilder : ICypherBuilder
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        
        public ICypherBuilder AddDetachDelete(string query)
        {
            AppendLine($"DETACH DELETE {query}");
            
            return this;
        }

        public ICypherBuilder AddDelete(string query)
        {
            AppendLine($"DELETE {query}");
            
            return this;
        }
        public ICypherBuilder AddUnwind(string query)
        {
            AppendLine($"UNWIND {query}");
            
            return this;
        }

        public ICypherBuilder AddMatch(string query)
        {
            AppendLine($"MATCH({query})");
            
            return this;
        }

        public ICypherBuilder AddMerge(string query)
        {
            AppendLine($"MERGE({query})");
            
            return this;
        }

        public ICypherBuilder AddWhere(string query)
        {
            AppendLine($"WHERE {query}");
            
            return this;
        }

        public ICypherBuilder AddCreate(string query)
        {
            AppendLine($"CREATE {query}");
            
            return this;
        }

        public ICypherBuilder AddSet(string query)
        {
            AppendLine($"SET {query}");
            
            return this;
        }

        private void AppendLine(string line)
        {
            _stringBuilder.AppendLine(line);
        }

        public override string ToString()
        { 
            return _stringBuilder.ToString();
        }
    }
}
