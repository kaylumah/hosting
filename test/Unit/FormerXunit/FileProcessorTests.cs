// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
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
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class FileProcessorTests
    {
        const string Root = "_site";

        static MockFileData EmptyFile()
        {
            return ContentFile(string.Empty);
        }

        static MockFileData WithFrontMatter(Dictionary<string, object> data = null)
        {
            string frontMatter = CreateFrontMatter(data);
            return ContentFile(frontMatter);
        }

        static MockFileData ContentFile(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            return new MockFileData(bytes);
        }

        [Fact]
        public async Task Test_FileProcessor_ChangedFileExtension()
        {
            SiteInfo optionsMock = new SiteInfo();
            Mock<ILogger<FileProcessor>> loggerMock = new Mock<ILogger<FileProcessor>>();

            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/test.md", EmptyFile() }
        });
            YamlFrontMatterMetadataProvider metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
            FileProcessor sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, metadataProviderMock, new MetadataParserOptions()
            {
                ExtensionMapping = new Dictionary<string, string> {
                        { ".md", ".html" }
                    }
            });
            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".md" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            BinaryFile testFile = result.Single(x => x.Name.Equals("test.html"));
        }

        [Fact]
        public async Task Test_FileProcessor_Subdirectories()
        {
            SiteInfo optionsMock = new SiteInfo();
            Mock<ILogger<FileProcessor>> loggerMock = new Mock<ILogger<FileProcessor>>();
            YamlFrontMatterMetadataProvider metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/_subdir/test.txt", EmptyFile() }
        });
            FileProcessor sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, metadataProviderMock, new MetadataParserOptions());
            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".txt" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            BinaryFile testFile = result.Single(x => x.Name.Equals("test.txt"));
            //testFile.MetaData.Collection.Should().Be("subdir");
        }

        [Fact]
        public async Task Test_FileProcessor_FrontMatter()
        {
            SiteInfo optionsMock = new SiteInfo();
            Mock<ILogger<FileProcessor>> loggerMock = new Mock<ILogger<FileProcessor>>();

            YamlFrontMatterMetadataProvider metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/a.txt", EmptyFile() },
            { $"{Root}/b.txt", WithFrontMatter() },
            { $"{Root}/c.txt", WithFrontMatter(new Dictionary<string, object> { { "tags", new string[] { "A" } }}) },
            { $"{Root}/d.txt", WithFrontMatter(new Dictionary<string, object> { }) }
        });
            FileProcessor sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, metadataProviderMock, new MetadataParserOptions());
            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".txt" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(4);

            BinaryFile fileA = result.Single(x => x.Name.Equals("a.txt"));
            BinaryFile fileB = result.Single(x => x.Name.Equals("b.txt"));
            BinaryFile fileC = result.Single(x => x.Name.Equals("c.txt"));
            BinaryFile fileD = result.Single(x => x.Name.Equals("d.txt"));

            fileA.MetaData.Count.Should().Be(1);
            fileB.MetaData.Count.Should().Be(1);
            fileC.MetaData.Count.Should().Be(2, "Default keys + one new keys equals 2");
            fileD.MetaData.Count.Should().Be(1);
        }

        [Fact(Skip = "figure out empty directory")]
        public async Task Test_FileProcessor_WithoutFiles_Should_ReturnEmptyList()
        {
            SiteInfo optionsMock = new SiteInfo();
            Mock<ILogger<FileProcessor>> loggerMock = new Mock<ILogger<FileProcessor>>();
            YamlFrontMatterMetadataProvider metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
            MockFileSystem mockFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> { });
            FileProcessor sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, metadataProviderMock, new MetadataParserOptions());
            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { }
            });
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Test_FileProcessor_WithoutFilter_Should_ReturnEmptyList()
        {
            SiteInfo optionsMock = new SiteInfo();
            Mock<ILogger<FileProcessor>> loggerMock = new Mock<ILogger<FileProcessor>>();
            YamlFrontMatterMetadataProvider metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/index.html", EmptyFile() }
        });
            FileProcessor sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, metadataProviderMock, new MetadataParserOptions());
            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
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
            SiteInfo optionsMock = new SiteInfo();
            Mock<ILogger<FileProcessor>> loggerMock = new Mock<ILogger<FileProcessor>>();
            YamlFrontMatterMetadataProvider metadataProviderMock = new YamlFrontMatterMetadataProvider(new YamlParser());
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/index.html", EmptyFile() },
            { $"{Root}/other.png", EmptyFile() }
        });
            FileProcessor sut = new FileProcessor(mockFileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock, metadataProviderMock, new MetadataParserOptions());
            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".html" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
        }

        [Fact]
        public void Test_FilemetadataParser_EmptyFileWithoutConfigOnlyGetsDefaultValues()
        {
            MockFileSystem mockFileSystem = new MockFileSystem();
            ILogger<FileProcessor> logger = NullLoggerFactory.Instance.CreateLogger<FileProcessor>();
            IContentPreprocessorStrategy[] preprocessorStrategies = Array.Empty<IContentPreprocessorStrategy>();
            SiteInfo siteInfo = new SiteInfo();
            IYamlParser yamlParser = new YamlParser();
            YamlFrontMatterMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(yamlParser);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();
            FileProcessor sut = new FileProcessor(mockFileSystem, logger, preprocessorStrategies, siteInfo, metadataProvider, metadataParserOptions);

            MetadataCriteria criteria = new MetadataCriteria()
            {
                Content = string.Empty,
                FileName = "file.html"
            };
            ParsedFile<FileMetaData> result = sut.Parse(criteria);
            result.Should().NotBeNull();
            result.FrontMatter.Should().NotBeNull();
            result.FrontMatter.Count.Should().Be(1, "Only URI is added by default");
            result.FrontMatter.Uri.Should().NotBeNull();
            result.FrontMatter.Uri.Should().Be("file.html");
        }

        //         [Fact]
        //         public void Test_FilemetadataParser_EmptyFileWithoutConfigOnlyGetsDefaultValues()
        //         {
        //             // Arange
        //             MetadataParserOptions optionsMock = new MetadataParserOptions();
        //             Mock<IFrontMatterMetadataProvider> metadataProviderMock = new Mock<IFrontMatterMetadataProvider>();

        //             metadataProviderMock
        //                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
        //                 .Returns(new ParsedFile<FileMetaData>(null, null));

        //             LoggerMock<FileParser> loggerMock = new LoggerMock<FileParser>();
        //             FileParser sut = new FileParser(loggerMock.Object, metadataProviderMock.Object, optionsMock);
        //             MetadataCriteria criteria = new MetadataCriteria
        //             {
        //                 Content = string.Empty,
        //                 FileName = "file.html"
        //             };

        //             // Act
        //             ParsedFile<FileMetaData> result = sut.Parse(criteria);

        //             // Assert
        //             result.Should().NotBeNull();
        //             result.FrontMatter.Should().NotBeNull();
        //             result.FrontMatter.Count.Should().Be(1, "Only URI is added by default");
        //             result.FrontMatter.Uri.Should().NotBeNull();
        //             result.FrontMatter.Uri.Should().Be("file.html");
        //         }

        //         [Fact]
        //         public void Test_FilemetadataParser_EmptyFileWithConfigThatIsEmptyOnlyGetsDefaultValues()
        //         {
        //             // Arange
        //             MetadataParserOptions options = new MetadataParserOptions
        //             {
        //                 Defaults = new DefaultMetadatas {
        //                     new DefaultMetadata {
        //                         Path = string.Empty,
        //                         Extensions = [ ".html" ],
        //                         Values = new FileMetaData {}
        //                     }
        //                 }
        //             };
        //             Mock<IFrontMatterMetadataProvider> metadataProviderMock = new Mock<IFrontMatterMetadataProvider>();

        //             metadataProviderMock
        //                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
        //                 .Returns(new ParsedFile<FileMetaData>(null, null));

        //             Mock<ILogger<FileParser>> loggerMock = new Mock<ILogger<FileParser>>();
        //             FileParser sut = new FileParser(loggerMock.Object, metadataProviderMock.Object, options);
        //             MetadataCriteria criteria = new MetadataCriteria
        //             {
        //                 Content = string.Empty,
        //                 FileName = "file.html"
        //             };

        //             // Act
        //             ParsedFile<FileMetaData> result = sut.Parse(criteria);

        //             // Assert
        //             result.Should().NotBeNull();
        //             result.FrontMatter.Should().NotBeNull();
        //             result.FrontMatter.Count.Should().Be(1, "Only URI is added by default");
        //             result.FrontMatter.Uri.Should().NotBeNull();
        //             result.FrontMatter.Uri.Should().Be("file.html");
        //         }

        //         [Fact]
        //         public void Test_FilemetadataParser_EmptyFileWithConfigTGetsDefaultValuesAndConfiguration()
        //         {
        //             // Arange
        //             MetadataParserOptions options = new MetadataParserOptions
        //             {
        //                 Defaults = new DefaultMetadatas {
        //                     new DefaultMetadata {
        //                         Path = string.Empty,
        //                         Extensions = [ ".html" ],
        //                         Values = new FileMetaData {
        //                             Layout = "default.html"
        //                         }
        //                     }
        //                 }
        //             };
        //             Mock<IFrontMatterMetadataProvider> metadataProviderMock = new Mock<IFrontMatterMetadataProvider>();

        //             metadataProviderMock
        //                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
        //                 .Returns(new ParsedFile<FileMetaData>(null, null));

        //             Mock<ILogger<FileParser>> loggerMock = new Mock<ILogger<FileParser>>();
        //             FileParser sut = new FileParser(loggerMock.Object, metadataProviderMock.Object, options);
        //             MetadataCriteria criteria = new MetadataCriteria
        //             {
        //                 Content = string.Empty,
        //                 FileName = "file.html"
        //             };

        //             // Act
        //             ParsedFile<FileMetaData> result = sut.Parse(criteria);

        //             // Assert
        //             result.Should().NotBeNull();
        //             result.FrontMatter.Should().NotBeNull();
        //             result.FrontMatter.Count.Should().Be(2, "Defaults = 1 + Applied Config = 1, Makes 2 values");
        //             result.FrontMatter.Uri.Should().NotBeNull();
        //             result.FrontMatter.Uri.Should().Be("file.html");
        //             result.FrontMatter.Layout.Should().NotBeNull();
        //             result.FrontMatter.Layout.Should().Be("default.html");
        //         }

        //         [Fact]
        //         public void Test_FilemetadataParser_EmptyFileWithConfigTGetsDefaultValuesAndMultipleConfigurations()
        //         {
        //             // Arange
        //             MetadataParserOptions options = new MetadataParserOptions
        //             {
        //                 Defaults = new DefaultMetadatas {
        //                     new DefaultMetadata {
        //                         Path = string.Empty,
        //                         Extensions = [ ".html" ],
        //                         Values = new FileMetaData {
        //                             Layout = "default.html"
        //                         }
        //                     },
        //                     new DefaultMetadata {
        //                         Path = "test",
        //                         Extensions = [ ".html" ],
        //                         Values = new FileMetaData {
        //                             Collection = "test"
        //                         }
        //                     }
        //                 }
        //             };
        //             Mock<IFrontMatterMetadataProvider> metadataProviderMock = new Mock<IFrontMatterMetadataProvider>();
        //             FileMetaData data = new FileMetaData
        //             {
        //                 OutputLocation = "test/:name:ext"
        //             };
        //             metadataProviderMock
        //                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
        //                 .Returns(new ParsedFile<FileMetaData>(null, data));

        //             Mock<ILogger<FileParser>> loggerMock = new Mock<ILogger<FileParser>>();
        //             FileParser sut = new FileParser(loggerMock.Object, metadataProviderMock.Object, options);
        //             MetadataCriteria criteria = new MetadataCriteria
        //             {
        //                 Content = string.Empty,
        //                 FileName = "test/file.html"
        //             };

        //             // Act
        //             ParsedFile<FileMetaData> result = sut.Parse(criteria);

        //             // Assert
        //             result.Should().NotBeNull();
        //             result.FrontMatter.Should().NotBeNull();
        //             result.FrontMatter.Count.Should().Be(3, "Defaults = 1 + Applied Config = 2, Makes 3 values");
        //             result.FrontMatter.Uri.Should().NotBeNull();
        //             result.FrontMatter.Uri.Should().Be("test/file.html");
        //             result.FrontMatter.Layout.Should().NotBeNull();
        //             result.FrontMatter.Layout.Should().Be("default.html");
        //             result.FrontMatter.Collection.Should().NotBeNull();
        //             result.FrontMatter.Collection.Should().Be("test");
        //         }

        //         [Fact]
        //         public void Test_FilemetadataParser_EmptyFileIfMultipleConfigurationsApplyLastOneWins()
        //         {
        //             // Arange
        //             MetadataParserOptions options = new MetadataParserOptions
        //             {
        //                 Defaults = new DefaultMetadatas {
        //                     new DefaultMetadata {
        //                         Path = string.Empty,
        //                         Extensions = [ ".html" ],
        //                         Values = new FileMetaData {
        //                             Layout = "default.html"
        //                         }
        //                     },
        //                     new DefaultMetadata {
        //                         Path = "test",
        //                         Extensions = [ ".html" ],
        //                         Values = new FileMetaData {
        //                             Layout = "other.html",
        //                             Collection = "test"
        //                         }
        //                     }
        //                 }
        //             };
        //             Mock<IFrontMatterMetadataProvider> metadataProviderMock = new Mock<IFrontMatterMetadataProvider>();

        //             FileMetaData meta = new FileMetaData()
        //             {
        //                 OutputLocation = "test/:name:ext"
        //             };
        //             metadataProviderMock
        //                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
        //                 .Returns(new ParsedFile<FileMetaData>(null, meta));

        //             LoggerMock<FileParser> loggerMock = new LoggerMock<FileParser>();
        //             FileParser sut = new FileParser(loggerMock.Object, metadataProviderMock.Object, options);
        //             MetadataCriteria criteria = new MetadataCriteria
        //             {
        //                 Content = string.Empty,
        //                 FileName = "test/file.html"
        //             };

        //             // Act
        //             ParsedFile<FileMetaData> result = sut.Parse(criteria);

        //             // Assert
        //             result.Should().NotBeNull();
        //             result.FrontMatter.Should().NotBeNull();
        //             result.FrontMatter.Count.Should().Be(3, "Defaults = 1 + Applied Config = 2, Makes 3 values");
        //             result.FrontMatter.Uri.Should().NotBeNull();
        //             result.FrontMatter.Uri.Should().Be("test/file.html");
        //             result.FrontMatter.Layout.Should().NotBeNull();
        //             result.FrontMatter.Layout.Should().Be("other.html");
        //             result.FrontMatter.Collection.Should().NotBeNull();
        //             result.FrontMatter.Collection.Should().Be("test");
        //         }

        //         [Fact]
        //         public void Test_FilemetadataParser_MultipleLayers()
        //         {
        //             // Arange
        //             MetadataParserOptions options = new MetadataParserOptions
        //             {
        //                 Defaults = new DefaultMetadatas {
        //                     new DefaultMetadata {
        //                         Path = string.Empty,
        //                         Extensions = [ ".html" ],
        //                         Values = new FileMetaData {
        //                             Layout = "default.html"
        //                         }
        //                     },
        //                     new DefaultMetadata {
        //                         Path = "test",
        //                         Extensions = [ ".html" ],
        //                         Values = new FileMetaData {
        //                             Collection = "test"
        //                         }
        //                     }
        //                 }
        //             };
        //             Mock<IFrontMatterMetadataProvider> metadataProviderMock = new Mock<IFrontMatterMetadataProvider>();

        //             FileMetaData data = new FileMetaData
        //             {
        //                 OutputLocation = "posts/2021/:name:ext"
        //             };

        //             metadataProviderMock
        //                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
        //                 .Returns(new ParsedFile<FileMetaData>(null, data));

        //             Mock<ILogger<FileParser>> loggerMock = new Mock<ILogger<FileParser>>();
        //             FileParser sut = new FileParser(loggerMock.Object, metadataProviderMock.Object, options);
        //             MetadataCriteria criteria = new MetadataCriteria
        //             {
        //                 Content = string.Empty,
        //                 FileName = "posts/2021/file.html"
        //             };

        //             // Act
        //             ParsedFile<FileMetaData> result = sut.Parse(criteria);

        //             // Assert
        //             result.Should().NotBeNull();
        //             result.FrontMatter.Should().NotBeNull();
        //             result.FrontMatter.Count.Should().Be(2, "Defaults = 1 + Applied Config = 1, Makes 2 values");
        //             result.FrontMatter.Uri.Should().NotBeNull();
        //             result.FrontMatter.Uri.Should().Be("posts/2021/file.html");
        //             result.FrontMatter.Layout.Should().NotBeNull();
        //             result.FrontMatter.Layout.Should().Be("default.html");
        //         }

        static string CreateFrontMatter(Dictionary<string, object> data = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("---");
            if (data != null)
            {
                string raw = new YamlDotNet.Serialization.Serializer().Serialize(data);
                stringBuilder.Append(raw);
            }

            stringBuilder.AppendLine("---");
            return stringBuilder.ToString();
        }

        string CreateEmptyXml()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = false,
                Encoding = Encoding.UTF8
            };
            MemoryStream stream = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("feed");
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
            }

            stream.Position = 0;
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }
}
