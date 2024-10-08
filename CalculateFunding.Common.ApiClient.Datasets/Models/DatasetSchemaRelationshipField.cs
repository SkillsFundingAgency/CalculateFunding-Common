﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetSchemaRelationshipField
    {
        public string Name { get; set; }

        public string SourceName { get; set; }

        public string SourceRelationshipName { get; set; }

        public bool IsAggregable { get; set; }

        public bool IsNumeric { get; set; }
        public string FullyQualifiedSourceName
        {
            get
            {
                return $"Datasets.{SourceRelationshipName}.{SourceName}";
            }
        }
    }
}
