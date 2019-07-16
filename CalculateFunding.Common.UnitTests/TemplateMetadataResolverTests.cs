using CalculateFunding.Common.TemplateMetadata;
using CalculateFunding.Common.TemplateMetadata.Schema10;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.UnitTests
{
    [TestClass]
    public class TemplateMetadataResolverTests
    {
        private const string _schemaVersion = "1.0";

        [TestMethod]
        public void TemplateMetadataResolver_GivenGeneratorRegisteredCorrectly_ReturnsCorrectGenerator()
        {
            //Arrange
            TemplateMetadataResolver templateMetadataResolver = CreateTemplateResolver();

            //Act
            templateMetadataResolver.Register(_schemaVersion, CreateTemplateGenerator(_schemaVersion));

            bool contains = templateMetadataResolver.Contains(_schemaVersion);

            ITemplateMetadataGenerator generator = templateMetadataResolver.GetService(_schemaVersion);

            //Assert
            contains
                .Should()
                .Be(true);

            generator
                .Should()
                .BeOfType<TemplateMetadataGenerator>();
        }

        [TestMethod]
        public void TemplateMetadataResolver_GivenGeneratorRegisteredInCorrectly_NoGeneratorReturned()
        {
            //Arrange
            Exception exception = null;

            TemplateMetadataResolver templateMetadataResolver = CreateTemplateResolver();

            //Act
            templateMetadataResolver.Register("2.0", CreateTemplateGenerator(_schemaVersion));

            bool contains = templateMetadataResolver.Contains(_schemaVersion);
            
            try
            {
                ITemplateMetadataGenerator generator = templateMetadataResolver.GetService(_schemaVersion);
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            //Assert
            contains
                .Should()
                .Be(false);

            exception.Message
                    .Should()
                    .Be($"Unable to find a registered resolver for schema version : {_schemaVersion}");
        }

        [TestMethod]
        public void TemplateMetadataResolver_GivenGeneratorRegisteredInCorrectly_TryGetReturnsTrue()
        {
            //Arrange
            TemplateMetadataResolver templateMetadataResolver = CreateTemplateResolver();

            ITemplateMetadataGenerator registeredGenerator = CreateTemplateGenerator(_schemaVersion);

            //Act
            templateMetadataResolver.Register(_schemaVersion, registeredGenerator);

            bool contains = templateMetadataResolver.TryGetService(_schemaVersion, out ITemplateMetadataGenerator generator);

            //Assert
            contains
                .Should()
                .Be(true);

            generator
                .Should()
                .Be(generator);
        }

        [TestMethod]
        public void TemplateMetadataResolver_GivenGeneratorRegisteredInCorrectly_TryGetReturnsFalse()
        {
            //Arrange
            TemplateMetadataResolver templateMetadataResolver = CreateTemplateResolver();

            //Act
            templateMetadataResolver.Register("2.0", CreateTemplateGenerator(_schemaVersion));

            bool contains = templateMetadataResolver.TryGetService(_schemaVersion, out ITemplateMetadataGenerator generator);

            //Assert
            contains
                .Should()
                .Be(false);

            generator
                .Should()
                .BeNull();
        }

        public ITemplateMetadataGenerator CreateTemplateGenerator(string specificationVersion, ILogger logger = null)
        {
            switch(specificationVersion)
            {
                case _schemaVersion:
                    {
                        return new TemplateMetadataGenerator(logger ?? CreateLogger());
                    }
                default:
                    {
                        return null;
                    }
            }
        }

        public ILogger CreateLogger()
        {
            return Substitute.For<ILogger>();
        }

        public TemplateMetadataResolver CreateTemplateResolver()
        {
            return new TemplateMetadataResolver();
        }
    }
}
