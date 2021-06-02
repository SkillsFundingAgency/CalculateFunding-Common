using CalculateFunding.Generators.Funding.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Testing;

namespace CalculateFunding.Generators.Funding.UnitTests
{
    [TestClass]
    public class FundingGeneratorTests
    {
        private Assembly _resourceAssembly;
        
        private FundingGenerator _generator;

        [TestInitialize]
        public void Setup()
        {
            _resourceAssembly = typeof(FundingGeneratorTests).Assembly;
            
            _generator = new FundingGenerator();
        }

        [TestMethod]
        public void WhenGeneratingFundingLineValuesForACollectionOfFundingLines_FundingStreamAndLineTotalsAreReturned()
        {
            IEnumerable<FundingLine> fundingLines = GetEmbeddedFundingLines("CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLines.json");

            FundingValue fundingValue = _generator.GenerateFundingValue(fundingLines, 3);

            fundingValue.TotalValue
                .Should()
                .Be(8100.632M);

            fundingValue.FundingLines.First().Value
                .Should()
                .Be(15900.632M);//showing the configurable rounding dp as this has precision of 5 in the test json

            fundingValue.FundingLines.First().FundingLines.First().Value
                .Should()
                .Be(7800M);

            fundingValue.FundingLines.First().FundingLines.First().FundingLines.First().Value
                .Should()
                .Be(7800M);

            fundingValue.FundingLines.First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(3100M);

            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.First().Value
                .Should()
                .Be(400M);

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
            IEnumerable<FundingLine> fundingLines = GetEmbeddedFundingLines("CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLinesWithNullCalculation.json");

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
            IEnumerable<FundingLine> fundingLines = GetEmbeddedFundingLines("CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLinesWithAllNullCalculations.json");

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

        private IEnumerable<FundingLine> GetEmbeddedFundingLines(string resourceName)
            => _resourceAssembly.GetEmbeddedResourceFileContents(resourceName).AsPoco<IEnumerable<FundingLine>>();
    }
}
