using System.Collections.Generic;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetVersions : Reference
    {
        public DatasetVersions()
        {
            Versions = new List<DatasetVersionModel>();
        }

        public string Description { get; set; }

        public int? SelectedVersion { get; set; }

        public IEnumerable<DatasetVersionModel> Versions { get; set; }
    }
}