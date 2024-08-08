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
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Xunit;

namespace Test.Unit.FormerXunit
{
    public class FileProcessorTests
    {
        const string Root = "_site";
        const int DefaultValueCount = 4;

        [Fact]
        public async Task Test_FileProcessor_ChangedFileExtension()
        {
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/test.md", EmptyFile() }
        });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions()
            {
                ExtensionMapping = new Dictionary<string, string> {
                        { ".md", ".html" }
                    }
            };

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".html" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            BinaryFile testFile = result.Single(x => x.Name.Equals("test.html"));
        }

        [Fact]
        public async Task Test_FileProcessor_Subdirectories()
        {
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/_subdir/test.txt", EmptyFile() }
        });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

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
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/a.txt", EmptyFile() },
            { $"{Root}/b.txt", WithFrontMatter() },
            { $"{Root}/c.txt", WithFrontMatter(new Dictionary<string, object> { { "tags", new string[] { "A" } }}) },
            { $"{Root}/d.txt", WithFrontMatter(new Dictionary<string, object> { }) }
        });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".txt" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(DefaultValueCount);

            BinaryFile fileA = result.Single(x => x.Name.Equals("a.txt"));
            BinaryFile fileB = result.Single(x => x.Name.Equals("b.txt"));
            BinaryFile fileC = result.Single(x => x.Name.Equals("c.txt"));
            BinaryFile fileD = result.Single(x => x.Name.Equals("d.txt"));

            fileA.MetaData.Count.Should().Be(DefaultValueCount);
            fileB.MetaData.Count.Should().Be(DefaultValueCount);
            fileC.MetaData.Count.Should().Be(DefaultValueCount + 1, "Default keys + one new keys equals 2");
            fileD.MetaData.Count.Should().Be(DefaultValueCount);
        }

        [Fact(Skip = "figure out empty directory")]
        public async Task Test_FileProcessor_WithoutFiles_Should_ReturnEmptyList()
        {
            MockFileSystem mockFileSystem = new MockFileSystem(
                new Dictionary<string, MockFileData> { });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

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
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/index.html", EmptyFile() }
        });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

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
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { $"{Root}/index.html", EmptyFile() },
            { $"{Root}/other.png", EmptyFile() }
        });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

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
        public async Task Test_FilemetadataParser_EmptyFileWithoutConfigOnlyGetsDefaultValues()
        {
            Dictionary<string, MockFileData> files = new()
            {
                [$"{Root}/file.html"] = string.Empty
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(DefaultValueCount, "Only URI is added by default");
            result.Uri.Should().NotBeNull();
            result.Uri.Should().Be("file.html");
        }

        [Fact]
        public async Task Test_FilemetadataParser_EmptyFileWithConfigThatIsEmptyOnlyGetsDefaultValues()
        {
            Dictionary<string, MockFileData> files = new()
            {
                [$"{Root}/file.html"] = string.Empty
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions
            {
                Defaults = new DefaultMetadatas {
                    new DefaultMetadata {
                        Path = string.Empty,
                        Extensions = [ ".html" ],
                        Values = new FileMetaData {}
                    }
                }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(DefaultValueCount, "Only URI is added by default");
            result.Uri.Should().NotBeNull();
            result.Uri.Should().Be("file.html");
        }

        [Fact]
        public async Task Test_FilemetadataParser_EmptyFileWithConfigTGetsDefaultValuesAndConfiguration()
        {
            Dictionary<string, MockFileData> files = new()
            {
                [$"{Root}/file.html"] = string.Empty
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions
            {
                Defaults = new DefaultMetadatas {
                            new DefaultMetadata {
                                Path = string.Empty,
                                Extensions = [ ".html" ],
                                Values = new FileMetaData {
                                    Layout = "default.html"
                                }
                            }
                        }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(DefaultValueCount + 1, "Defaults = 1 + Applied Config = 1, Makes 2 values");
            result.Uri.Should().NotBeNull();
            result.Uri.Should().Be("file.html");
            result.Layout.Should().NotBeNull();
            result.Layout.Should().Be("default.html");
        }

        [Fact]
        public async Task Test_FilemetadataParser_EmptyFileWithConfigTGetsDefaultValuesAndMultipleConfigurations()
        {
            Dictionary<string, MockFileData> files = new()
            {
                [$"{Root}/test/file.html"] = "---\r\noutputlocation: test/:name:ext---"
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions
            {
                Defaults = new DefaultMetadatas {
                            new DefaultMetadata {
                                Path = string.Empty,
                                Extensions = [ ".html" ],
                                Values = new FileMetaData {
                                    Layout = "default.html"
                                }
                            },
                            new DefaultMetadata {
                                Path = "test",
                                Extensions = [ ".html" ],
                                Values = new FileMetaData {
                                    Collection = "test"
                                }
                            }
                        }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(DefaultValueCount + 2, "Defaults = 1 + Applied Config = 2, Makes 3 values");
            result.Uri.Should().NotBeNull();
            result.Uri.Should().Be("test/file.html");
            result.Layout.Should().NotBeNull();
            result.Layout.Should().Be("default.html");
            result.Collection.Should().NotBeNull();
            result.Collection.Should().Be("test");
        }

        [Fact]
        public async Task Test_FilemetadataParser_EmptyFileIfMultipleConfigurationsApplyLastOneWins()
        {
            Dictionary<string, MockFileData> files = new()
            {
                [$"{Root}/test/file.html"] = "---\r\noutputlocation: test/:name:ext---"
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions
            {
                Defaults = new DefaultMetadatas {
                            new DefaultMetadata {
                                Path = string.Empty,
                                Extensions = [ ".html" ],
                                Values = new FileMetaData {
                                    Layout = "default.html"
                                }
                            },
                            new DefaultMetadata {
                                Path = "test",
                                Extensions = [ ".html" ],
                                Values = new FileMetaData {
                                    Layout = "other.html",
                                    Collection = "test"
                                }
                            }
                        }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(DefaultValueCount + 2, "Defaults = 1 + Applied Config = 2, Makes 3 values");
            result.Uri.Should().NotBeNull();
            result.Uri.Should().Be("test/file.html");
            result.Layout.Should().NotBeNull();
            result.Layout.Should().Be("other.html");
            result.Collection.Should().NotBeNull();
            result.Collection.Should().Be("test");
        }

        [Fact]
        public async Task Test_FilemetadataParser_MultipleLayers()
        {
            Dictionary<string, MockFileData> files = new()
            {
                [$"posts/2021/file.html"] = "---\r\noutputlocation: posts/2021/:name:ext---"
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions
            {
                Defaults = new DefaultMetadatas {
                            new DefaultMetadata {
                                Path = string.Empty,
                                Extensions = [ ".html" ],
                                Values = new FileMetaData {
                                    Layout = "default.html"
                                }
                            },
                            new DefaultMetadata {
                                Path = "test",
                                Extensions = [ ".html" ],
                                Values = new FileMetaData {
                                    Collection = "test"
                                }
                            }
                        }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "posts",
                DirectoriesToSkip = new string[] { },
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(DefaultValueCount + 1, "Defaults = 1 + Applied Config = 1, Makes 2 values");
            result.Uri.Should().NotBeNull();
            result.Uri.Should().Be("posts/2021/file.html");
            result.Layout.Should().NotBeNull();
            result.Layout.Should().Be("default.html");
        }

        FileProcessor CreateFileProcessor(MockFileSystem mockFileSystem, MetadataParserOptions metadataParserOptions, IContentPreprocessorStrategy[] preprocessorStrategies = null, SiteInfo siteInfo = null)
        {
            preprocessorStrategies ??= Array.Empty<IContentPreprocessorStrategy>();
            siteInfo ??= new SiteInfo();
            ILogger<FileProcessor> logger = NullLoggerFactory.Instance.CreateLogger<FileProcessor>();
            IYamlParser yamlParser = new YamlParser();
            YamlFrontMatterMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(yamlParser);
            FileProcessor sut = new FileProcessor(mockFileSystem, logger, preprocessorStrategies, siteInfo, metadataProvider, metadataParserOptions);
            return sut;
        }

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
    }
}