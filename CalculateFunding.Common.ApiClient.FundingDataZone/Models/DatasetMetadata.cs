using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.FundingDataZone.Models
{
    public class DatasetMetadata : Dataset
    {
        public IEnumerable<FieldMetadata> Fields { get; set; }
    }
}
