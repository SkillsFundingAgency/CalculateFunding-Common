using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetVersions : Reference
    {
        public DatasetVersions()
        {
            Versions = Enumerable.Empty<int>();
        }

        public int? SelectedVersion { get; set; }

        public IEnumerable<int> Versions { get; set; }
    }
}