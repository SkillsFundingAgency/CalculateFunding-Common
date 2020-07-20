namespace CalculateFunding.Common.ApiClient.FDZ.Models
{
    public class FieldMetadata
    {
        public string Name { get; set; }

        public FieldType FieldType { get; set; }

        public bool Required { get; set; }

        public bool IsTableKey { get; set; }
    }
}
