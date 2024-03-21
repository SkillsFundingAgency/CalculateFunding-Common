using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class UpdateFDSDatasetSchemaVersionModel
    {
        public string RelationshipId { get; set; }

        public string DatasetDefintionId { get; set; }

        public List<DatasetComparisonField> RemovedFields { get; set; }
    }
}
