// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Microsoft.Extensions.Logging;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Xunit;

namespace Test.Unit;

public class FileProcessorTests
{
    private const string root = "_site";

    private static MockFileData EmptyFile()
    {
        return ContentFile(string.Empty);
    }

    private static MockFileData WithFrontMatter(Dictionary<string, object> data = null)
    {
        var frontMatter = CreateFrontMatter(data);
        return ContentFile(frontMatter);
    }

    private static MockFileData ContentFile(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MockFileData(bytes);
    }


    [Fact]
    public async Task Test_FileProcessor_ChangedFileExtension()
    {
        var optionsMock = new SiteInfo();
        var loggerMock = new Mock<ILogger<FileProcessor>>();

        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{root}/test.md", EmptyFile() }
        });
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock,
            new MetadataParserOptions()
            {
                ExtensionMapping = new Dictionary<string, string> {
                        { ".md", ".html" }
                }
            }
        );
        var sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
            RootDirectory = "_site",
            DirectoriesToSkip = new string[] { },
            FileExtensionsToTarget = new string[] { ".md" }
        });
        result.Should().NotBeEmpty();
        result.Count().Should().Be(1);
        var testFile = result.Single(x => x.Name.Equals("test.html"));
    }

    [Fact]
    public async Task Test_FileProcessor_Subdirectories()
    {
        var optionsMock = new SiteInfo();
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{root}/_subdir/test.txt", EmptyFile() }
        });
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, new MetadataParserOptions());
        var sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
            RootDirectory = "_site",
            DirectoriesToSkip = new string[] { },
            FileExtensionsToTarget = new string[] { ".txt" }
        });
        result.Should().NotBeEmpty();
        result.Count().Should().Be(1);
        var testFile = result.Single(x => x.Name.Equals("test.txt"));
        //testFile.MetaData.Collection.Should().Be("subdir");
    }

    [Fact]
    public async Task Test_FileProcessor_FrontMatter()
    {
        var optionsMock = new SiteInfo();
        var loggerMock = new Mock<ILogger<FileProcessor>>();

        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{root}/a.txt", EmptyFile() },
            { $"{root}/b.txt", WithFrontMatter() },
            { $"{root}/c.txt", WithFrontMatter(new Dictionary<string, object> { { "tags", new string[] { "A" } }}) },
            { $"{root}/d.txt", WithFrontMatter(new Dictionary<string, object> { }) }

        });
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, new MetadataParserOptions());
        var sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
            RootDirectory = "_site",
            DirectoriesToSkip = new string[] { },
            FileExtensionsToTarget = new string[] { ".txt" }
        });
        result.Should().NotBeEmpty();
        result.Count().Should().Be(4);

        var fileA = result.Single(x => x.Name.Equals("a.txt"));
        var fileB = result.Single(x => x.Name.Equals("b.txt"));
        var fileC = result.Single(x => x.Name.Equals("c.txt"));
        var fileD = result.Single(x => x.Name.Equals("d.txt"));

        fileA.MetaData.Count.Should().Be(1);
        fileB.MetaData.Count.Should().Be(1);
        fileC.MetaData.Count.Should().Be(2, "Default keys + one new keys equals 2");
        fileD.MetaData.Count.Should().Be(1);
    }

    [Fact(Skip = "figure out empty directory")]
    public async Task Test_FileProcessor_WithoutFiles_Should_ReturnEmptyList()
    {
        var optionsMock = new SiteInfo();
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var mockFileSystem = new MockFileSystem(
            new Dictionary<string, MockFileData> { });
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, new MetadataParserOptions());
        var sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
            DirectoriesToSkip = new string[] { },
            FileExtensionsToTarget = new string[] { }
        });
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Test_FileProcessor_WithoutFilter_Should_ReturnEmptyList()
    {
        var optionsMock = new SiteInfo();
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{root}/index.html", EmptyFile() }
        });
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, new MetadataParserOptions());
        var sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
            RootDirectory = "_site",
            DirectoriesToSkip = new string[] { },
            FileExtensionsToTarget = new string[] { }
        });
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Test_FileProcessor_WithFilter_Should_ReturnMatchingFiles()
    {
        var optionsMock = new SiteInfo();
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{root}/index.html", EmptyFile() },
            { $"{root}/other.png", EmptyFile() }
        });
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, new MetadataParserOptions());
        var sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
            RootDirectory = "_site",
            DirectoriesToSkip = new string[] { },
            FileExtensionsToTarget = new string[] { ".html" }
        });
        result.Should().NotBeEmpty();
        result.Count().Should().Be(1);
    }

    private static string CreateFrontMatter(Dictionary<string, object> data = null)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("---");
        if (data != null)
        {
            var raw = new YamlDotNet.Serialization.Serializer().Serialize(data);
            stringBuilder.Append(raw);
        }
        stringBuilder.AppendLine("---");
        return stringBuilder.ToString();
    }

    private string CreateEmptyXml()
    {
        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = false,
            Encoding = Encoding.UTF8
        };
        var stream = new MemoryStream();
        using (XmlWriter writer = XmlWriter.Create(stream, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("feed");
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }
        stream.Position = 0;
        var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }
}
