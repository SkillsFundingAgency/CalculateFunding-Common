namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class PublishedSpecificationItem
    {
        public uint TemplateId { get; set; }

        public string Name { get; set; }

        public string SourceCodeName { get; set; }

        public FieldType FieldType { get; set; }

        public bool IsObsolete { get; set; }

        public bool IsSelected { get; set; }

        public bool IsUsedInCalculation { get; set; }

    }
}