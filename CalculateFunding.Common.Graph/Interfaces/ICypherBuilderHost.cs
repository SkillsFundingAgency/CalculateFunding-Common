using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface ICypherBuilderHost
    {
        ICypherBuilder Current();
    }
}
