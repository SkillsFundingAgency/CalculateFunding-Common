using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class ConverterDataMergeLog
    {
        public string Id { get; set; }

        public IEnumerable<RowCopyResult> Results { get; set; }
        
        public ConverterMergeRequest Request { get; set; }
        
        public string JobId { get; set; }
        
        public int DatasetVersionCreated { get; set; }  
    }
}