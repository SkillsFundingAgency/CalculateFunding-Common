﻿namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationCreateModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string SpecificationId { get; set; }

        public CalculationValueType? ValueType { get; set; }

        public string SourceCode { get; set; }

        public string Description { get; set; }
    }
}
