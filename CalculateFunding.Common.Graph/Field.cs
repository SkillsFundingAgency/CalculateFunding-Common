using CalculateFunding.Common.Graph.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Graph
{
    public class Field : IField
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
