﻿using System.Collections.Generic;

 namespace CalculateFunding.Common.ApiClient.Providers.Models
{
    public class ProviderGraphQlFieldInfo
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public IEnumerable<string> EnumValues { get; set; }
        public string FieldTypeKind { get; set; }
    }
}