using CalculateFunding.Common.Models;
using System;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetDefinitionViewModel : Reference
    {
        public string Description { get; set; }

        public int Version { get; set; }

        public string LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedDt { get; set; }

        public bool IsLatestVersion { get; set; }

        public string Comment { get; set; }
    }
}