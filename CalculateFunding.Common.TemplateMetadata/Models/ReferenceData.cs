using CalculateFunding.Common.TemplateMetadata.Enums;

namespace CalculateFunding.Common.TemplateMetadata.Models
{
    public class ReferenceData
    {
        public uint TemplateReferenceId { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        public ReferenceDataValueFormat Format { get; set; }

        public AggregationType AggregationType { get; set; }
    }
}