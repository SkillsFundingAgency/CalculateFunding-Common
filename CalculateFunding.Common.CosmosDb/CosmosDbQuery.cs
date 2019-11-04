using System.Collections.Generic;
using Microsoft.Azure.Cosmos;

namespace CalculateFunding.Common.CosmosDb
{
    public class CosmosDbQuery
    {
        public string QueryText { get; set; }

        public IEnumerable<CosmosDbQueryParameter> Parameters { get; set; }

        public QueryDefinition CosmosQueryDefinition
        {
            get
            {
                QueryDefinition queryDefinition = new QueryDefinition(QueryText);

                if (Parameters != null)
                {
                    foreach (var parameter in Parameters)
                    {
                        queryDefinition = queryDefinition.WithParameter(parameter.Name, parameter.Value);
                    }
                }

                return queryDefinition;
            }
        }

        public CosmosDbQuery() { }

        public CosmosDbQuery(string queryText)
        {
            QueryText = queryText;
        }

        public CosmosDbQuery(string queryText, IEnumerable<CosmosDbQueryParameter> parameters)
        {
            QueryText = queryText;
            Parameters = parameters;
        }
    }
}
