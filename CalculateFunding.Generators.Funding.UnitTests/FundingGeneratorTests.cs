using CalculateFunding.Generators.Funding.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
                .Be(8200.632M);

            fundingValue.FundingLines.First().Value
                .Should()
                .Be(16200.632M);//showing the configurable rounding dp as this has precision of 5 in the test json

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
                .Be(5000.632M);

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
