using System;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetVersionResponseViewModel : Reference
    {
        public string BlobName { get; set; }

        public int Version { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public PublishStatus PublishStatus { get; set; }

        public Reference Definition { get; set; }

        public string Description { get; set; }

        public Reference Author { get; set; }

        public string Comment { get; set; }

        public int CurrentDataSourceRows { get; set; }

        public int PreviousDataSourceRows { get; set; }
    }
}