using CalculateFunding.Common.TemplateMetadata.Models;

namespace CalculateFunding.Common.TemplateMetadata
{
    public interface ITemplateMetadataGenerator
    {
        TemplateMetadataContents GetMetadata(string templateContents);
    }
}
