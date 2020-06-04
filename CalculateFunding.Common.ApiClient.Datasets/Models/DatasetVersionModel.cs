using CalculateFunding.Common.Models;
using System;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetVersionModel
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public Reference Author { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
