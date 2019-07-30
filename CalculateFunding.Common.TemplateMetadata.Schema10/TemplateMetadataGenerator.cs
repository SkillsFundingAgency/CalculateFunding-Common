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

        /// <summary>
        /// Get the meta contents required given a JSON template
        /// </summary>
        /// <param name="templateContents">Template JSON</param>
        /// <returns>Metadata content required</returns>
        public TemplateMetadataContents GetMetadata(string templateContents)
        {
            Guard.IsNullOrWhiteSpace(templateContents, nameof(templateContents));

            FeedBaseModel feedBaseModel = GetFeed(templateContents);

            if (feedBaseModel != null && !feedBaseModel.Funding.FundingValue.FundingValueByDistributionPeriod.IsNullOrEmpty())
            {
                IEnumerable<Models.FundingLine> FundingLines = feedBaseModel.Funding.FundingValue.FundingValueByDistributionPeriod.FirstOrDefault()?.FundingLines;

                TemplateMetadataContents contents = new TemplateMetadataContents
                {
                    RootFundingLines = FundingLines?.Select(x => GetFundingLines(new TemplateMetadata.Models.FundingLine
                    {
                        Name = x.Name,
                        ReferenceId = x.TemplateLineId,
                        FundingLineCode = x.FundingLineCode,
                        Type = (FundingLineType)Enum.Parse(typeof(FundingLineType), x.Type.ToString())
                    },
                            x.FundingLines)
                    ),
                    SchemaVersion = feedBaseModel.SchemaVersion
                };

                return contents;
            }

            return null;
        }

        public override ValidationResult Validate(ValidationContext<string> context)
        {
            FeedBaseModel feedBaseModel = GetFeed(context.InstanceToValidate);

            return feedBaseModel == null ? new ValidationResult(new[] { new ValidationFailure("Template", "Instance cannot be null") }) : _templateMetadataValidator.Validate(feedBaseModel);
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

        private TemplateMetadata.Models.FundingLine GetFundingLines(TemplateMetadata.Models.FundingLine fundingLine, IEnumerable<Models.FundingLine> fundingLines)
        {
            if (!fundingLines.IsNullOrEmpty())
            {
                List<TemplateMetadata.Models.FundingLine> fundingLinesList = new List<TemplateMetadata.Models.FundingLine> { };

                foreach (Models.FundingLine fundingLineMap in fundingLines)
                {
                    TemplateMetadata.Models.FundingLine fundingLineLocal = new TemplateMetadata.Models.FundingLine
                    {
                        Name = fundingLineMap.Name,
                        ReferenceId = fundingLineMap.TemplateLineId,
                        FundingLineCode = fundingLineMap.FundingLineCode,
                        Type = (FundingLineType)Enum.Parse(typeof(FundingLineType), fundingLineMap.Type.ToString())
                    };

                    fundingLineLocal.Calculations = fundingLineMap.Calculations?.Select(calculationMap => new TemplateMetadata.Models.Calculation
                    {
                        Name = calculationMap.Name,
                        ValueFormat = (CalculationValueFormat)Enum.Parse(typeof(CalculationValueFormat), calculationMap.ValueFormat.ToString()),
                        AggregationType = (AggregationType)Enum.Parse(typeof(AggregationType), calculationMap.AggregationType.ToString()),
                        Type = (CalculationType)Enum.Parse(typeof(CalculationType), calculationMap.Type.ToString()),
                        TemplateCalculationId = calculationMap.TemplateCalculationId,
                        ReferenceData = calculationMap.ReferenceData.Select(x => new TemplateMetadata.Models.ReferenceData
                        {
                            Name = x.Name,
                            TemplateReferenceId = x.TemplateReferenceId,
                            AggregationType = (AggregationType)Enum.Parse(typeof(AggregationType), x.AggregationType.ToString()),

                        })
                    });

                    GetFundingLines(fundingLineLocal, fundingLineMap.FundingLines);
                    fundingLinesList.Add(fundingLineLocal);
                }

                fundingLine.FundingLines = fundingLinesList;
            }

            return fundingLine;
        }
    }
}
