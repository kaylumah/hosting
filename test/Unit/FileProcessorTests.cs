// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Test.Utilities;
using Xunit;

namespace Test.Unit;

public class FileProcessorTests
{
    [Fact]
    public async Task Test_FileProcessor_ChangedFileExtension()
    {
        var root = "/a/b/c";
        var optionsMock = Options.Create(new SiteInfo()
        {
        });
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var fileProviderMock = new Mock<IFileProvider>()
            .SetupFileProviderMock(
                root,
                new List<FakeDirectory>()
                {
                        new FakeDirectory(string.Empty, new FakeFile[] {
                            new FakeFile("test.md")
                        })
                });
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var fileSystem = new FileSystem(fileProviderMock.Object, metadataProviderMock);
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock,
            Options.Create(new MetadataParserOptions()
            {
                ExtensionMapping = new Dictionary<string, string> {
                        { ".md", ".html" }
                }
            })
        );
        var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
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
        var root = "/a/b/c";
        var optionsMock = Options.Create(new SiteInfo() { });
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var fileProviderMock = new Mock<IFileProvider>()
            .SetupFileProviderMock(
                root,
                new List<FakeDirectory>()
                {
                        new FakeDirectory("_subdir", new FakeFile[] {
                            new FakeFile(Path.Combine(root, "test.txt"))
                        })
                });
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var fileSystem = new FileSystem(fileProviderMock.Object, metadataProviderMock);
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, Options.Create(new MetadataParserOptions()));
        var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
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
        var root = "/a/b/c";
        var optionsMock = Options.Create(new SiteInfo() { });
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var fileProviderMock = new Mock<IFileProvider>()
            .SetupFileProviderMock(
                root,
                new List<FakeDirectory>() {
                        new FakeDirectory(string.Empty, new FakeFile[] {
                            // File without any frontmatter
                            new FakeFile("a.txt", Encoding.UTF8.GetBytes(string.Empty)),
                            // File with empty frontmatter
                            new FakeFile("b.txt", Encoding.UTF8.GetBytes(
                                CreateFrontMatter()
                            )),
                            // File with frontmatter but only new keys
                            new FakeFile("c.txt", Encoding.UTF8.GetBytes(
                                CreateFrontMatter(new Dictionary<string, object> {
                                    { "tags", new string[] { "A" } }
                                })
                            )),
                            // File writh frontmatter, but overwritten keys
                            new FakeFile("d.txt", Encoding.UTF8.GetBytes(
                                CreateFrontMatter(new Dictionary<string, object> {

                                })
                            )),
                            /*new FakeFile("2021-01-01-article.txt", Encoding.UTF8.GetBytes(
                                CreateFrontMatter(new Dictionary<string, object>{
                                    { "PublishedTime", "18:00" }
                                })
                            ))*/
                        })
                }
            );
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var fileSystem = new FileSystem(fileProviderMock.Object, metadataProviderMock);
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, Options.Create(new MetadataParserOptions()));
        var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
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

    [Fact]
    public async Task Test_FileProcessor_WithoutFiles_Should_ReturnEmptyList()
    {
        var root = "/a/b/c";
        var optionsMock = Options.Create(new SiteInfo() { });
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var fileProviderMock = new Mock<IFileProvider>()
            .SetupFileProviderMock(
                root,
                new List<FakeDirectory>() { }
            );
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var fileSystem = new FileSystem(fileProviderMock.Object, metadataProviderMock);
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, Options.Create(new MetadataParserOptions()));
        var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
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
        var root = "/a/b/c";
        var optionsMock = Options.Create(new SiteInfo() { });
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var fileProviderMock = new Mock<IFileProvider>()
            .SetupFileProviderMock(
                root,
                new List<FakeDirectory>() {
                        new FakeDirectory(string.Empty, new FakeFile[] {
                            new FakeFile("index.html")
                        })
                }
            );
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var fileSystem = new FileSystem(fileProviderMock.Object, metadataProviderMock);
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, Options.Create(new MetadataParserOptions()));
        var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
            DirectoriesToSkip = new string[] { },
            FileExtensionsToTarget = new string[] { }
        });
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Test_FileProcessor_WithFilter_Should_ReturnMatchingFiles()
    {
        var root = "/a/b/c";
        var optionsMock = Options.Create(new SiteInfo() { });
        var loggerMock = new Mock<ILogger<FileProcessor>>();
        var fileProviderMock = new Mock<IFileProvider>()
            .SetupFileProviderMock(
                root,
                new List<FakeDirectory>() {
                        new FakeDirectory(string.Empty, new FakeFile[] {
                            new FakeFile("index.html"),
                            new FakeFile("other.png")
                        })
                }
            );
        var metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
        var fileSystem = new FileSystem(fileProviderMock.Object, metadataProviderMock);
        var fileMetadataParserMock = new FileMetadataParser(new Mock<ILogger<FileMetadataParser>>().Object, metadataProviderMock, Options.Create(new MetadataParserOptions()));
        var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, fileMetadataParserMock);
        var result = await sut.Process(new FileFilterCriteria
        {
            DirectoriesToSkip = new string[] { },
            FileExtensionsToTarget = new string[] { ".html" }
        });
        result.Should().NotBeEmpty();
        result.Count().Should().Be(1);
    }

    private string CreateFrontMatter(Dictionary<string, object> data = null)
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
        using (System.Xml.XmlWriter writer = XmlWriter.Create(stream, settings))
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