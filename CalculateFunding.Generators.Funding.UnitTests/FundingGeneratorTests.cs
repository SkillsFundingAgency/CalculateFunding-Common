using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CalculateFunding.Common.Testing;
using CalculateFunding.Generators.Funding.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace CalculateFunding.Generators.Funding.UnitTests
{
    [TestClass]
    public class FundingGeneratorTests
    {
        private FundingGenerator _generator;

        [TestInitialize]
        public void Setup()
        {
            _generator = new FundingGenerator();
        }

        [TestMethod]
        public void WhenGeneratingFundingLineValuesForACollectionOfFundingLines_FundingStreamAndLineTotalsAreReturned()
        {
            IEnumerable<FundingLine> fundingLines = JsonConvert.DeserializeObject<IEnumerable<FundingLine>>(GetResourceString($"CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLines.json"));

            FundingValue fundingValue = _generator.GenerateFundingValue(fundingLines, 3);

            fundingValue.TotalValue
                .Should()
                .Be(10200.634M);

            fundingValue.FundingLines.First().Value
                .Should()
                .Be(18200.634M);//showing the configurable rounding dp as this has precision of 5 in the test json

            fundingValue.FundingLines.First().FundingLines.First().Value
                .Should()
                .Be(8000M);

            fundingValue.FundingLines.First().FundingLines.First().FundingLines.First().Value
                .Should()
                .Be(8000M);

            fundingValue.FundingLines.First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(3200M);

            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.First().Value
                .Should()
                .Be(500M);

            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(1200M);

            fundingValue.FundingLines.First().FundingLines.Skip(2).First().Value
                .Should()
                .Be(7000.634M);

            fundingValue.FundingLines.First().FundingLines.Skip(2).First().FundingLines.First().Value
                .Should()
                .Be(2000.001M);

            fundingValue.FundingLines.First().FundingLines.Skip(3).First().Value
                .Should()
                .Be(null);
        }

        [TestMethod]
        public void WhenGeneratingFundingLineWithMixOfNullAndValuesForACollectionOfFundingLines_FundingStreamAndLineTotalsAreReturned()
        {
            IEnumerable<FundingLine> fundingLines = JsonConvert.DeserializeObject<IEnumerable<FundingLine>>(GetResourceString($"CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLinesWithNullCalculation.json"));

            FundingValue fundingValue = _generator.GenerateFundingValue(fundingLines);

            // "funding line 4" > "calculation 5" = 10
            fundingValue.TotalValue
                .Should()
                .Be(10M);
            fundingValue.FundingLines.First().Value
                .Should()
                .Be(10M);

            // "funding line 2"
            fundingValue.FundingLines.First().FundingLines.First().Value
                .Should()
                .Be(null);

            // "funding line 3" -> mix of null and 0 in values = 0
            fundingValue.FundingLines.First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(0);

            // "funding line 7"
            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.First().Value
                .Should()
                .Be(null);

            // "funding line 8" -> null + 0 = 0
            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(0);

            // "calculation 5"
            fundingValue.FundingLines.First().FundingLines.Skip(2).First().Value
                .Should()
                .Be(10M);
        }

        [TestMethod]
        public void WhenGeneratingFundingLineAllNullValuesForACollectionOfFundingLines_FundingStreamAndLineTotalsAreReturned()
        {
            IEnumerable<FundingLine> fundingLines = JsonConvert.DeserializeObject<IEnumerable<FundingLine>>(GetResourceString($"CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLinesWithAllNullCalculations.json"));

            FundingValue fundingValue = _generator.GenerateFundingValue(fundingLines);

            // "funding line 4" > "calculation 5" = 10
            fundingValue.TotalValue
                .Should()
                .Be(null);

            fundingValue.FundingLines.First().Value
                .Should()
                .Be(null);

            // "funding line 2"
            fundingValue.FundingLines.First().FundingLines.First().Value
                .Should()
                .Be(null);

            // "funding line 3" -> mix of null and 0 in values = 0
            fundingValue.FundingLines.First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(null);

            // "funding line 7"
            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.First().Value
                .Should()
                .Be(null);

            // "funding line 8" -> null + 0 = 0
            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(null);

            // "calculation 5"
            fundingValue.FundingLines.First().FundingLines.Skip(2).First().Value
                .Should()
                .Be(null);
        }

        [TestMethod]
        public void WhenGeneratingFundingLineValuesForACollectionOfFundingLinesUsingAdjustmentCalculations_ExpectedRoundingInHierachyBasedOnFundingLineTotals()
        {
            IEnumerable<FundingLine> fundingLines = JsonConvert.DeserializeObject<IEnumerable<FundingLine>>(GetResourceString($"CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLines-rounding-adjustment.json"));

            FundingValue fundingValue = _generator.GenerateFundingValue(fundingLines, 2);

            fundingValue.TotalValue.Should().Be(60M);

            IEnumerable<FundingLine> allFundingLines = fundingValue.FundingLines.Flatten(_ => _.FundingLines);

            FundingLine aggregateFundingLine = allFundingLines.Single(_ => _.Name == "aggregate funding line");

            // This is important to check the total is rounded to sub funding lines, then the aggregate funding lines are summed.
            // If just the adjustment calculations are summed, then the aggregate would be 60.01, instead of 60.00.
            // This is because the adjustment calculations would be 5 lots of xx.001, which would equals xx.01, instead of the funding lines summed from xx.00
            aggregateFundingLine.Value.Should()
                .Be(60M);

            FundingLine fundingLine1 = allFundingLines.Single(_ => _.Name == "funding line 1");
            fundingLine1.Value.Should().Be(10M);

            FundingLine fundingLine2 = allFundingLines.Single(_ => _.Name == "funding line 2");
            fundingLine2.Value.Should().Be(10M);

            FundingLine fundingLine3 = allFundingLines.Single(_ => _.Name == "funding line 3");
            fundingLine3.Value.Should().Be(11M);

            FundingLine fundingLine4 = allFundingLines.Single(_ => _.Name == "funding line 4");
            fundingLine4.Value.Should().Be(11M);

            FundingLine fundingLine5 = allFundingLines.Single(_ => _.Name == "funding line 5");
            fundingLine5.Value.Should().Be(12M);

            FundingLine fundingLine6 = allFundingLines.Single(_ => _.Name == "funding line 6");
            fundingLine6.Value.Should().Be(12M);

            FundingLine fundingLine7 = allFundingLines.Single(_ => _.Name == "funding line 7");
            fundingLine7.Value.Should().Be(13M);

            FundingLine fundingLine8 = allFundingLines.Single(_ => _.Name == "funding line 8");
            fundingLine8.Value.Should().Be(13M);

            FundingLine fundingLine9 = allFundingLines.Single(_ => _.Name == "funding line 9");
            fundingLine9.Value.Should().Be(14M);

            FundingLine fundingLine10 = allFundingLines.Single(_ => _.Name == "funding line 10");
            fundingLine10.Value.Should().Be(14M);
        }

        [TestMethod]
        public void WhenGeneratingFundingLineValuesForACollectionOfFundingLinesUsingCashCalculations_ExpectedRoundingInHierachyBasedOnFundingLineTotals()
        {
            IEnumerable<FundingLine> fundingLines = JsonConvert.DeserializeObject<IEnumerable<FundingLine>>(GetResourceString($"CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLines-rounding.json"));

            FundingValue fundingValue = _generator.GenerateFundingValue(fundingLines, 2);

            fundingValue.TotalValue.Should().Be(60M);

            IEnumerable<FundingLine> allFundingLines = fundingValue.FundingLines.Flatten(_ => _.FundingLines);

            FundingLine aggregateFundingLine = allFundingLines.Single(_ => _.Name == "aggregate funding line");

            // This is important to check the total is rounded to sub funding lines, then the aggregate funding lines are summed.
            // If just the cash calculations are summed, then the aggregate would be 60.01, instead of 60.00.
            // This is because the cash calculations would be 5 lots of xx.001, which would equals xx.01, instead of the funding lines summed from xx.00
            aggregateFundingLine.Value.Should()
                .Be(60M);

            FundingLine fundingLine1 = allFundingLines.Single(_ => _.Name == "funding line 1");
            fundingLine1.Value.Should().Be(10M);

            FundingLine fundingLine2 = allFundingLines.Single(_ => _.Name == "funding line 2");
            fundingLine2.Value.Should().Be(10M);

            FundingLine fundingLine3 = allFundingLines.Single(_ => _.Name == "funding line 3");
            fundingLine3.Value.Should().Be(11M);

            FundingLine fundingLine4 = allFundingLines.Single(_ => _.Name == "funding line 4");
            fundingLine4.Value.Should().Be(11M);

            FundingLine fundingLine5 = allFundingLines.Single(_ => _.Name == "funding line 5");
            fundingLine5.Value.Should().Be(12M);

            FundingLine fundingLine6 = allFundingLines.Single(_ => _.Name == "funding line 6");
            fundingLine6.Value.Should().Be(12M);

            FundingLine fundingLine7 = allFundingLines.Single(_ => _.Name == "funding line 7");
            fundingLine7.Value.Should().Be(13M);

            FundingLine fundingLine8 = allFundingLines.Single(_ => _.Name == "funding line 8");
            fundingLine8.Value.Should().Be(13M);

            FundingLine fundingLine9 = allFundingLines.Single(_ => _.Name == "funding line 9");
            fundingLine9.Value.Should().Be(14M);

            FundingLine fundingLine10 = allFundingLines.Single(_ => _.Name == "funding line 10");
            fundingLine10.Value.Should().Be(14M);
        }

        private string GetResourceString(string resourcePath)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                    throw new InvalidOperationException(
                        $"Could not load manifest resource stream from {Assembly.GetExecutingAssembly().FullName} at requested path {resourcePath}");

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
