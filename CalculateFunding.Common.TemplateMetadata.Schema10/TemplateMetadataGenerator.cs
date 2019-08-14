using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema10.Models;
using CalculateFunding.Common.TemplateMetadata.Schema10.Validators;
using CalculateFunding.Common.Utility;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Schema10FundingLine = CalculateFunding.Common.TemplateMetadata.Models.FundingLine;

namespace CalculateFunding.Common.TemplateMetadata.Schema10
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
            FeedBaseModel feedBaseModel = GetFeed(context.InstanceToValidate);

            return feedBaseModel == null ? new ValidationResult(new[] { new ValidationFailure("Template", "Instance cannot be null") }) : _templateMetadataValidator.Validate(feedBaseModel);
        }

        /// <summary>
        /// Get the meta contents required given a JSON template
        /// </summary>
        /// <param name="templateContents">Template JSON</param>
        /// <returns>Metadata content required</returns>
        public TemplateMetadataContents GetMetadata(string templateContents)
        {
            Guard.IsNullOrWhiteSpace(templateContents, nameof(templateContents));

            FeedBaseModel feedBaseModel = GetFeed(templateContents);

            if (feedBaseModel != null && !feedBaseModel.Funding.ProviderFundings.IsNullOrEmpty())
            {
                IEnumerable<Models.FundingLine> FundingLines = feedBaseModel.Funding.ProviderFundings.FirstOrDefault()?.FundingValue.FundingLines;

                TemplateMetadataContents contents = new TemplateMetadataContents
                {
                    RootFundingLines = FundingLines?.Select(x => ToFundingLine(x)),
                    SchemaVersion = feedBaseModel.SchemaVersion
                };

                return contents;
            }

            return null;
        }

        private static Schema10FundingLine ToFundingLine(Models.FundingLine source)
        {
            return new Schema10FundingLine
            {
                Name = source.Name,
                TemplateLineId = source.TemplateLineId,
                FundingLineCode = source.FundingLineCode,
                Type = (FundingLineType)Enum.Parse(typeof(FundingLineType), source.Type.ToString()),
                Calculations = source.Calculations?.Select(calculationMap => ToCalculation(calculationMap)),
                FundingLines = source.FundingLines?.Select(x => ToFundingLine(x)),
                Value = source.Value
            };
        }

        private static TemplateMetadata.Models.Calculation ToCalculation(Models.Calculation source)
        {
            return new TemplateMetadata.Models.Calculation
            {
                Name = source.Name,
                ValueFormat = (CalculationValueFormat)Enum.Parse(typeof(CalculationValueFormat), source.ValueFormat.ToString()),
                AggregationType = (AggregationType)Enum.Parse(typeof(AggregationType), source.AggregationType.ToString()),
                Type = (CalculationType)Enum.Parse(typeof(CalculationType), source.Type.ToString()),
                TemplateCalculationId = source.TemplateCalculationId,
                ReferenceData = source.ReferenceData.Select(x => new TemplateMetadata.Models.ReferenceData
                {
                    Name = x.Name,
                    TemplateReferenceId = x.TemplateReferenceId,
                    AggregationType = (AggregationType)Enum.Parse(typeof(AggregationType), x.AggregationType.ToString()),

                }),
                Calculations = source.Calculations?.Select(x => ToCalculation(x))
            };
        }

        private FeedBaseModel GetFeed(string templateContents)
        {
            try
            {
                return JsonConvert.DeserializeObject<FeedBaseModel>(templateContents);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to deserialize template : {ex.Message}");
                return null;
            }
        }
    }
}
