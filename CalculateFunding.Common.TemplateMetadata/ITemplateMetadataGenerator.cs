using CalculateFunding.Common.TemplateMetadata.Models;
using FluentValidation;

namespace CalculateFunding.Common.TemplateMetadata
{
    public interface ITemplateMetadataGenerator : IValidator<string>
    {
        TemplateMetadataContents GetMetadata(string templateContents);
    }
}
