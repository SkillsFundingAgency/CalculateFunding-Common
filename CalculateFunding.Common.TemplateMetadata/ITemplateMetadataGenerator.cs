using CalculateFunding.Common.TemplateMetadata.Models;
using FluentValidation;

namespace CalculateFunding.Common.TemplateMetadata
{
    public interface ITemplateMetadataGenerator : IValidator
    {
        TemplateMetadataContents GetMetadata(string templateContents);
    }
}
