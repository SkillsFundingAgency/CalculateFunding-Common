using CalculateFunding.Common.Graph.Interfaces;
using System.Collections.Generic;
using System.Linq;
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

        public ICypherBuilder AddMatch(params IMatch[] matchess)
        {
            AppendLine($"MATCH {string.Join(",", Matches(matchess))}");
            
            return this;
        }

        private IEnumerable<string> Matches(IMatch[] matches)
        {
            return matches.Select(x =>
            {
                Match match = x as Match;
                if (match != null)
                {
                    return $"{match.Pattern}";
                }
                else
                {
                    MatchWithAlias alias = x as MatchWithAlias;

                    return $"{alias.Alias}={alias.Pattern}";
                }
            });
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

        public ICypherBuilder AddReturn(string[] returns)
        {
            AppendLine($"RETURN {string.Join(",", returns)}");

            return this;
        }

        public ICypherBuilder AddAnd(string query)
        {
            AppendLine($"AND {query}");

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
