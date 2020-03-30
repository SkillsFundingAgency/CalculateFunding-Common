using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    public class Relationship
    {
        public dynamic One { get; set; }
        public dynamic Two { get; set; }
        public string Type { get; set; }
    }
}
