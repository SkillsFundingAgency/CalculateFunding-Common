using System;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class Calculation : Reference
    {
        public string SpecificationId { get; set; }

        public string FundingStreamId { get; set; }

        public string SourceCode { get; set; }

        public CalculationType CalculationType { get; set; }

        public string SourceCodeName { get; set; }

        public CalculationNamespace Namespace { get; set; }

        public bool WasTemplateCalculation { get; set; }

        public CalculationValueType ValueType { get; set; }

        public DateTimeOffset? LastUpdated { get; set; }

        public Reference Author { get; set; }

        public int Version { get; set; }

        public PublishStatus PublishStatus { get; set; }
    }
}