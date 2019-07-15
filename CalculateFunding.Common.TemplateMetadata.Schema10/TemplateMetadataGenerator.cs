using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema10.Models;
using CalculateFunding.Common.Utility;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.TemplateMetadata.Schema10
{
    public class TemplateMetadataGenerator : ITemplateMetadataGenerator
    {
        ILogger _logger;

        public TemplateMetadataGenerator(ILogger logger)
        {
            Guard.ArgumentNotNull(logger, nameof(logger));

            _logger = logger;
        }

        /// <summary>
        /// Get the meta contents required given a JSON template
        /// </summary>
        /// <param name="templateContents">Template JSON</param>
        /// <returns>Metadata content required</returns>
        public TemplateMetadataContents GetMetadata(string templateContents)
        {
            Guard.IsNullOrWhiteSpace(templateContents, nameof(templateContents));

            try
            {
                FeedBaseModel feedBaseModel = JsonConvert.DeserializeObject<FeedBaseModel>(templateContents);

                if (!feedBaseModel.Funding.FundingValue.FundingValueByDistributionPeriod.IsNullOrEmpty())
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
                        )
                    };

                    return contents;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to deserialize template : {ex.Message}");
            }

            return null;
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
