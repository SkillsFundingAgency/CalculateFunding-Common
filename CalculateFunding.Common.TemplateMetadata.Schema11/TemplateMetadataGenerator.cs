using System;
using System.Linq;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema11.Mapping;
using CalculateFunding.Common.TemplateMetadata.Schema11.Models;
using CalculateFunding.Common.TemplateMetadata.Schema11.Validators;
using CalculateFunding.Common.Utility;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using Serilog;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace CalculateFunding.Common.TemplateMetadata.Schema11
{
    public class TemplateMetadataGenerator : AbstractValidator<string>, ITemplateMetadataGenerator
    {
        private readonly ILogger _logger;
        private readonly TemplateMetadataValidator _templateMetadataValidator;

        public TemplateMetadataGenerator(ILogger logger)
        {
            Guard.ArgumentNotNull(logger, nameof(logger));

            _logger = logger;
            _templateMetadataValidator = new TemplateMetadataValidator();
        }

        public override ValidationResult Validate(ValidationContext<string> context)
        {
            (SchemaJson feedBaseModel, string errorMessage) = GetFeed(context.InstanceToValidate);

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                if (feedBaseModel == null)
                {
                    return new ValidationResult(new[] {new ValidationFailure("Template", "Instance cannot be null")});
                }

                return _templateMetadataValidator.Validate(feedBaseModel);
            }
            else
            {
                return new ValidationResult(new[] {new ValidationFailure("Template", errorMessage)});
            }
        }

        /// <summary>
        /// Get the meta contents required given a JSON template
        /// </summary>
        /// <param name="templateContents">Template JSON</param>
        /// <returns>Metadata content required</returns>
        public TemplateMetadataContents GetMetadata(string templateContents)
        {
            Guard.IsNullOrWhiteSpace(templateContents, nameof(templateContents));

            (SchemaJson feedBaseModel, string errorMessage) = GetFeed(templateContents);

            if (feedBaseModel != null)
            {
                TemplateMetadataContents contents = new TemplateMetadataContents
                {
                    RootFundingLines = feedBaseModel.FundingStreamTemplate.FundingLines?.Select(x => x.ToFundingLine()),
                    SchemaVersion = feedBaseModel.SchemaVersion
                };

                return contents;
            }

            return null;
        }
        
        private (SchemaJson template, string errorMessage) GetFeed(string templateContents)
        {
            try
            {
                return (JsonConvert.DeserializeObject<SchemaJson>(templateContents), null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to deserialize template : {ex.Message}");
                return (null, ex.Message);
            }
        }
    }
}
