using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.FDZ.Models
{
    public class DatasetMetadata : Dataset
    {
        public IEnumerable<FieldMetadata> Fields { get; set; }
    }
}
