using System.IO;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Xunit;

namespace Test.Unit
{
    public class MetadataTests
    {
        [Fact]
        public void Test_FileMetadataParser_EmptyInput()
        {
            var root = "/a/b/c";
            var fileName = "1.txt";
            var filePath = Path.Combine(root, "b/d/e/f", fileName);
            var criteria = new MetadataCriteria
            {
                FileName = fileName,
                Content = string.Empty
            };

            var loggerMock = new Mock<ILogger<FileMetadataParser>>();
            var optionsMock = Options.Create(new MetadataParserOptions());
            IYamlParser yamlParser = new YamlParser();
            IMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(yamlParser);
            IFileMetadataParser sut = new FileMetadataParser(loggerMock.Object, metadataProvider, optionsMock);
            var result = sut.Parse(criteria);
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public void Test_FileMetadataParser_EmptyInput2()
        {
            var root = "/a/b/c";
            var fileName = "1.txt";
            var filePath = Path.Combine(root, fileName);
            var criteria = new MetadataCriteria
            {
                FileName = fileName,
                Content = string.Empty
            };

            var loggerMock = new Mock<ILogger<FileMetadataParser>>();
            var optionsMock = Options.Create(new MetadataParserOptions());
            IYamlParser yamlParser = new YamlParser();
            IMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(yamlParser);
            IFileMetadataParser sut = new FileMetadataParser(loggerMock.Object, metadataProvider, optionsMock);
            var result = sut.Parse(criteria);
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
        }
    }
}