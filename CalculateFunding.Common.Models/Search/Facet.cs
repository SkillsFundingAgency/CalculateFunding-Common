using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.Models.Search
{
    public class Facet
    {
        public Facet()
        {
            FacetValues = new List<FacetValue>();
        }

        public string Name { get; set; }

        public IEnumerable<FacetValue> FacetValues { get; set; }
    }
}
