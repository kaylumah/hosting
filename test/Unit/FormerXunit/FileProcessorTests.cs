﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
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

        [Fact]
        public async Task Test_FileProcessor_ChangedFileExtension()
        {
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{Root}/test.md", MockFileSystemHelper.EmptyFile() }
            });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions()
            {
                ExtensionMapping = new Dictionary<string, string>
                {
                    { ".md", ".html" }
                }
            };

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".html" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            BinaryFile testFile = result.Single(x => x.Name.Equals("test.html", StringComparison.Ordinal));
        }

        [Fact]
        public async Task Test_FileProcessor_Subdirectories()
        {
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{Root}/_subdir/test.txt", MockFileSystemHelper.EmptyFile() }
            });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".txt" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            BinaryFile testFile = result.Single(x => x.Name.Equals("test.txt", StringComparison.Ordinal));
            //testFile.MetaData.Collection.Should().Be("subdir");
        }

        [Fact]
        public async Task Test_FileProcessor_FrontMatter()
        {
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{Root}/a.txt", MockFileSystemHelper.EmptyFile() },
                { $"{Root}/b.txt", MockFileSystemHelper.WithFrontMatter() },
                { $"{Root}/c.txt", MockFileSystemHelper.WithFrontMatter(new Dictionary<string, object> { { "tags", new string[] { "A" } } }) },
                { $"{Root}/d.txt", MockFileSystemHelper.WithFrontMatter(new Dictionary<string, object> { }) }
            });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".txt" }
            });
            result.Should().NotBeEmpty();
            result.Count().Should().Be(4);

            BinaryFile fileA = result.Single(x => x.Name.Equals("a.txt", StringComparison.Ordinal));
            BinaryFile fileB = result.Single(x => x.Name.Equals("b.txt", StringComparison.Ordinal));
            BinaryFile fileC = result.Single(x => x.Name.Equals("c.txt", StringComparison.Ordinal));
            BinaryFile fileD = result.Single(x => x.Name.Equals("d.txt", StringComparison.Ordinal));

            fileA.MetaData.Count.Should().Be(1);
            fileB.MetaData.Count.Should().Be(1);
            fileC.MetaData.Count.Should().Be(2, "Default keys + one new keys equals 2");
            fileD.MetaData.Count.Should().Be(1);
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
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = Array.Empty<string>()
            });
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Test_FileProcessor_WithoutFilter_Should_ReturnEmptyList()
        {
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{Root}/index.html", MockFileSystemHelper.EmptyFile() }
            });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = Array.Empty<string>()
            });
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Test_FileProcessor_WithFilter_Should_ReturnMatchingFiles()
        {
            MockFileSystem mockFileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { $"{Root}/index.html", MockFileSystemHelper.EmptyFile() },
                { $"{Root}/other.png", MockFileSystemHelper.EmptyFile() }
            });
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> result = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = Array.Empty<string>(),
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
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(1, "Only URI is added by default");
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
                Defaults = new DefaultMetadatas
                {
                    new DefaultMetadata
                    {
                        Path = string.Empty,
                        Extensions = [".html"],
                        Values = new FileMetaData { }
                    }
                }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(1, "Only URI is added by default");
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
                Defaults = new DefaultMetadatas
                {
                    new DefaultMetadata
                    {
                        Path = string.Empty,
                        Extensions = [".html"],
                        Values = new FileMetaData
                        {
                            Layout = "default.html"
                        }
                    }
                }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(2, "Defaults = 1 + Applied Config = 1, Makes 2 values");
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
                Defaults = new DefaultMetadatas
                {
                    new DefaultMetadata
                    {
                        Path = string.Empty,
                        Extensions = [".html"],
                        Values = new FileMetaData
                        {
                            Layout = "default.html"
                        }
                    },
                    new DefaultMetadata
                    {
                        Path = "test",
                        Extensions = [".html"],
                        Values = new FileMetaData
                        {
                            Collection = "test"
                        }
                    }
                }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "_site",
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(3, "Defaults = 1 + Applied Config = 2, Makes 3 values");
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
                Defaults = new DefaultMetadatas
                {
                    new DefaultMetadata
                    {
                        Path = string.Empty,
                        Extensions = [".html"],
                        Values = new FileMetaData
                        {
                            Layout = "default.html"
                        }
                    },
                    new DefaultMetadata
                    {
                        Path = "test",
                        Extensions = [".html"],
                        Values = new FileMetaData
                        {
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
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(3, "Defaults = 1 + Applied Config = 2, Makes 3 values");
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
                Defaults = new DefaultMetadatas
                {
                    new DefaultMetadata
                    {
                        Path = string.Empty,
                        Extensions = [".html"],
                        Values = new FileMetaData
                        {
                            Layout = "default.html"
                        }
                    },
                    new DefaultMetadata
                    {
                        Path = "test",
                        Extensions = [".html"],
                        Values = new FileMetaData
                        {
                            Collection = "test"
                        }
                    }
                }
            };
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            IEnumerable<BinaryFile> processResult = await sut.Process(new FileFilterCriteria
            {
                RootDirectory = "posts",
                DirectoriesToSkip = Array.Empty<string>(),
                FileExtensionsToTarget = new string[] { ".html" }
            });
            BinaryFile targetFile = processResult.Single();
            FileMetaData result = targetFile.MetaData;
            result.Should().NotBeNull();
            result.Count.Should().Be(2, "Defaults = 1 + Applied Config = 1, Makes 2 values");
            result.Uri.Should().NotBeNull();
            result.Uri.Should().Be("posts/2021/file.html");
            result.Layout.Should().NotBeNull();
            result.Layout.Should().Be("default.html");
        }

        FileProcessor CreateFileProcessor(MockFileSystem mockFileSystem, MetadataParserOptions metadataParserOptions,
            IContentPreprocessorStrategy[]? preprocessorStrategies = null, SiteInfo? siteInfo = null)
        {
            preprocessorStrategies ??= Array.Empty<IContentPreprocessorStrategy>();
            siteInfo ??= new SiteInfo();
            ILogger<FileProcessor> logger = NullLoggerFactory.Instance.CreateLogger<FileProcessor>();
            IYamlParser yamlParser = new YamlParser();
            YamlFrontMatterMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(yamlParser);
            FileProcessor sut = new FileProcessor(mockFileSystem, logger, preprocessorStrategies, siteInfo, metadataProvider, metadataParserOptions);
            return sut;
        }
    }
}