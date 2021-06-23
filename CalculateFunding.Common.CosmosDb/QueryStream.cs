using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.CosmosDb
{
    public class QueryStream<T>
    {
        public IEnumerable<T> Documents { get; set; }
    }
}
