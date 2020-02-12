using CalculateFunding.Common.Graph.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Graph
{
    public class CypherBuilderHost : ICypherBuilderHost
    {
        public ICypherBuilder Current()
        {
            return new CypherBuilder();
        }
    }
}
