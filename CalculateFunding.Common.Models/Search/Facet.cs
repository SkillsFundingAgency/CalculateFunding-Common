using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.Models.Search
{
    public class Facet
    {
        public Facet()
        {
            FacetValues = Enumerable.Empty<FacetValue>();
        }

        public string Name { get; set; }

        public IEnumerable<FacetValue> FacetValues { get; set; }
    }
}
