using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetValidationStatusModel
    {
        public string OperationId { get; set; }

        public DatasetValidationStatus CurrentOperation { get; set; }

        public string ErrorMessage { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public string DatasetId { get; set; }

        public IDictionary<string, IEnumerable<string>> ValidationFailures { get; set; }
    }
}