// Copyright (c) Kaylumah, 2025. All rights reserved.
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
            Dictionary<string, MockFileData> fileSystemData = new Dictionary<string, MockFileData>
            {
                { $"{Root}/test.md", MockFileSystemHelper.EmptyFile() }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(fileSystemData);

            Dictionary<string, string> metaDataConfig = new Dictionary<string, string>
                {
                    { ".md", ".html" }
                };
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();
            metadataParserOptions.ExtensionMapping = metaDataConfig;

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".html" };
            IEnumerable<BinaryFile> result = await sut.Process(criteria);
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            BinaryFile testFile = result.Single(x => x.Name.Equals("test.html", StringComparison.Ordinal));
        }

        [Fact]
        public async Task Test_FileProcessor_Subdirectories()
        {
            Dictionary<string, MockFileData> mockSystemData = new Dictionary<string, MockFileData>
            {
                { $"{Root}/_subdir/test.txt", MockFileSystemHelper.EmptyFile() }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(mockSystemData);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".txt" };
            IEnumerable<BinaryFile> result = await sut.Process(criteria);
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
            BinaryFile testFile = result.Single(x => x.Name.Equals("test.txt", StringComparison.Ordinal));
            //testFile.MetaData.Collection.Should().Be("subdir");
        }

        [Fact]
        public async Task Test_FileProcessor_FrontMatter()
        {
            Dictionary<string, MockFileData> fileData = new Dictionary<string, MockFileData>
            {
                { $"{Root}/a.txt", MockFileSystemHelper.EmptyFile() },
                { $"{Root}/b.txt", MockFileSystemHelper.WithFrontMatter() },
                { $"{Root}/c.txt", MockFileSystemHelper.WithFrontMatter(new Dictionary<string, object> { { "tags", new string[] { "A" } } }) },
                { $"{Root}/d.txt", MockFileSystemHelper.WithFrontMatter(new Dictionary<string, object> { }) }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(fileData);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".txt" };
            IEnumerable<BinaryFile> result = await sut.Process(criteria);
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
            Dictionary<string, MockFileData> fileData = new Dictionary<string, MockFileData>();
            MockFileSystem mockFileSystem = new MockFileSystem(fileData);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = Array.Empty<string>();
            IEnumerable<BinaryFile> result = await sut.Process(criteria);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Test_FileProcessor_WithoutFilter_Should_ReturnEmptyList()
        {
            Dictionary<string, MockFileData> fileData = new Dictionary<string, MockFileData>
            {
                { $"{Root}/index.html", MockFileSystemHelper.EmptyFile() }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(fileData);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = Array.Empty<string>();
            IEnumerable<BinaryFile> result = await sut.Process(criteria);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Test_FileProcessor_WithFilter_Should_ReturnMatchingFiles()
        {
            Dictionary<string, MockFileData> fileData = new Dictionary<string, MockFileData>
            {
                { $"{Root}/index.html", MockFileSystemHelper.EmptyFile() },
                { $"{Root}/other.png", MockFileSystemHelper.EmptyFile() }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(fileData);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".html" };
            IEnumerable<BinaryFile> result = await sut.Process(criteria);
            result.Should().NotBeEmpty();
            result.Count().Should().Be(1);
        }

        [Fact]
        public async Task Test_FilemetadataParser_EmptyFileWithoutConfigOnlyGetsDefaultValues()
        {
            Dictionary<string, MockFileData> files = new Dictionary<string, MockFileData>{
                { $"{Root}/file.html", string.Empty }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);
            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".html" };
            IEnumerable<BinaryFile> processResult = await sut.Process(criteria);
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
                { $"{Root}/file.html", string.Empty }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);

            DefaultMetadata metadata = new DefaultMetadata();
            metadata.Path = string.Empty;
            metadata.Extensions = [".html"];
            metadata.Values = new FileMetaData();

            DefaultMetadatas defaults = new DefaultMetadatas
            {
                metadata
            };

            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();
            metadataParserOptions.Defaults = defaults;
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);

            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".html" };
            IEnumerable<BinaryFile> processResult = await sut.Process(criteria);
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
                { $"{Root}/file.html",  string.Empty }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);

            DefaultMetadata metadata = new DefaultMetadata();
            metadata.Path = string.Empty;
            metadata.Extensions = [".html"];
            metadata.Values = new FileMetaData();
            metadata.Values.Layout = "default.html";
            DefaultMetadatas defaults = new DefaultMetadatas
            {
                metadata
            };

            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();
            metadataParserOptions.Defaults = defaults;
        
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".html" };
            IEnumerable<BinaryFile> processResult = await sut.Process(criteria);
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
                { $"{Root}/test/file.html", "---\r\noutputlocation: test/:name:ext---" }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);

            DefaultMetadata metaA = new DefaultMetadata();
            metaA.Path = string.Empty;
            metaA.Extensions = [".html"];
            metaA.Values = new FileMetaData();
            metaA.Values.Layout = "default.html";

            DefaultMetadata metaB = new DefaultMetadata();
            metaB.Path = "test";
            metaB.Extensions = [".html"];
            metaB.Values = new FileMetaData();
            metaB.Values.Collection = "test";

            DefaultMetadatas defaults = new DefaultMetadatas
            {
                metaA,
                metaB
            };

            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();
            metadataParserOptions.Defaults = defaults;
            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".html" };
            IEnumerable<BinaryFile> processResult = await sut.Process(criteria);
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
                { $"{Root}/test/file.html",  "---\r\noutputlocation: test/:name:ext---" }
            };
            MockFileSystem mockFileSystem = new MockFileSystem(files);

            DefaultMetadata metaA = new DefaultMetadata();
            metaA.Path = string.Empty;
            metaA.Extensions = [".html"];
            metaA.Values = new FileMetaData();
            metaA.Values.Layout = "default.html";

            DefaultMetadata metaB = new DefaultMetadata();
            metaB.Path = "test";
            metaB.Extensions = [".html"];
            metaB.Values = new FileMetaData();
            metaB.Values.Layout = "other.html";
            metaB.Values.Collection = "test";

            MetadataParserOptions metadataParserOptions = new MetadataParserOptions();
            metadataParserOptions.Defaults = new DefaultMetadatas
            {
                metaA,
                metaB
            };

            FileProcessor sut = CreateFileProcessor(mockFileSystem, metadataParserOptions);
            FileFilterCriteria criteria = new FileFilterCriteria();
            criteria.RootDirectory = "_site";
            criteria.DirectoriesToSkip = Array.Empty<string>();
            criteria.FileExtensionsToTarget = new string[] { ".html" };
            IEnumerable<BinaryFile> processResult = await sut.Process(criteria);
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