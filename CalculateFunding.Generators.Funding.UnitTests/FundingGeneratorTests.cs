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
        public void WhenGeratingFundingLineValuesForACollectionOfFundingLines_FundingStreamAndLineTotalsAreReturned()
        {
            IEnumerable<FundingLine> fundingLines = JsonConvert.DeserializeObject<IEnumerable<FundingLine>>(GetResourceString($"CalculateFunding.Generators.Funding.UnitTests.Resources.exampleFundingLines.json"));

            FundingGenerator generator = new FundingGenerator();

            FundingValue fundingValue = _generator.GenerateFundingValue(fundingLines);

            fundingValue.TotalValue
                .Should()
                .Be(8500.63M);

            fundingValue.FundingLines.First().Value
                .Should()
                .Be(16500.63M);

            fundingValue.FundingLines.First().FundingLines.First().Value
                .Should()
                .Be(8000M);

            fundingValue.FundingLines.First().FundingLines.First().FundingLines.First().Value
                .Should()
                .Be(8000M);

            fundingValue.FundingLines.First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(3500M);

            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.First().Value
                .Should()
                .Be(500M);

            fundingValue.FundingLines.First().FundingLines.Skip(1).First().FundingLines.Skip(1).First().Value
                .Should()
                .Be(1500M);

            fundingValue.FundingLines.First().FundingLines.Skip(2).First().Value
                .Should()
                .Be(5000.63M);

            fundingValue.FundingLines.First().FundingLines.Skip(3).First().Value
                .Should()
                .Be(0M);
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
