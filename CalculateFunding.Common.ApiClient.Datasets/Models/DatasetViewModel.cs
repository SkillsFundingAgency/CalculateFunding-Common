using System.Collections.Generic;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetViewModel : Reference
    {
        public Reference Definition { get; set; }

        public string Description { get; set; }

        public IEnumerable<DatasetVersionViewModel> History { get; set; }
    }
}