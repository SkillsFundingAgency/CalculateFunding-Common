using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface IRelationship
    {
        dynamic One { get; set; }
        dynamic Two { get; set; }
        string Type { get; set; }
    }
}
