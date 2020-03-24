using CalculateFunding.Common.Graph.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.Graph
{
    public class MatchWithAlias : IMatch
    {
        public string Pattern { get; set; }
        public string Alias { get; set; }
    }
}
