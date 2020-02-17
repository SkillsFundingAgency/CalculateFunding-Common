using CalculateFunding.Common.Graph.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Graph
{
    public class CypherBuilderFactory : ICypherBuilderFactory
    {
        public ICypherBuilder NewCypherBuilder()
        {
            return new CypherBuilder();
        }
    }
}
