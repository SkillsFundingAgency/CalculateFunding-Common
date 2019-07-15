using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema10;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CalculateFunding.Common.UnitTests
{
    [TestClass]
    public class TemplateMetadataGeneratorTests
    {
        [TestMethod]
        public void TemplateMetadataSchema10_GetInvalidMetaData_ReturnsEmptyContents()
        {
            ILogger logger = CreateLogger();

            TemplateMetadataGenerator templateMetaDataGenerator = CreateTemplateGenerator(logger);
            TemplateMetadataContents contents = templateMetaDataGenerator.GetMetadata(GetResourceString("CalculateFunding.Common.UnitTests.Resources.dsg1.0.error.json"));

            logger
                .Received(1)
                .Error(Arg.Is<Exception>(x => x.GetType().Name == "JsonSerializationException"), Arg.Any<string>());
        }

        [TestMethod]
        public void TemplateMetadataSchema10_GetValidMetaData_ReturnsValidContents()
        {
            TemplateMetadataGenerator templateMetaDataGenerator = CreateTemplateGenerator();
            TemplateMetadataContents contents = templateMetaDataGenerator.GetMetadata(GetResourceString("CalculateFunding.Common.UnitTests.Resources.dsg1.0.json"));

            contents.RootFundingLines.Count()
                .Should()
                .Be(2);

            contents.RootFundingLines.First().Name
                .Should()
                .Be("PriorToRecoupment");

            contents.RootFundingLines.First().Type
                .Should()
                .Be(FundingLineType.Information);

            contents.RootFundingLines.Last().Name
                .Should()
                .Be("PostDeductionForRecoupmentAndHighNeeds");

            contents.RootFundingLines.Last().FundingLineCode
                .Should()
                .Be("DSG-001");

            contents.RootFundingLines.Last().FundingLines.First().FundingLines.First().Calculations.First().Name
                .Should()
                .Be("2019-20 mobility and premises funding");

            contents.RootFundingLines.Last().FundingLines.First().FundingLines.First().Calculations.First().ValueFormat
                .Should()
                .Be(CalculationValueFormat.Currency);

            contents.RootFundingLines.Last().FundingLines.First().FundingLines.First().Calculations.First().AggregationType
                .Should()
                .Be(AggregationType.Sum);

            contents.RootFundingLines.Last().FundingLines.First().FundingLines.First().Calculations.First().Type
                .Should()
                .Be(CalculationType.LumpSum);

            contents.RootFundingLines.Last().FundingLines.First().FundingLines.First().Calculations.First().ReferenceData.First().AggregationType
                .Should()
                .Be(AggregationType.Sum);

            contents.RootFundingLines.Last().FundingLines.First().FundingLines.First().Calculations.First().ReferenceData.First().Format
                .Should()
                .Be(ReferenceDataValueFormat.Number);
        }

        public TemplateMetadataGenerator CreateTemplateGenerator(ILogger logger = null)
        {
            return new TemplateMetadataGenerator(logger ?? CreateLogger());
        }

        public ILogger CreateLogger()
        {
            return Substitute.For<ILogger>();
        }

        public string GetResourceString(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
