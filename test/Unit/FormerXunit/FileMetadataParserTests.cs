// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

// using FluentAssertions;
// using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Ssg.Extensions.Metadata.Abstractions;
// using Test.Unit.FormerXunit.Mocks;
// using Xunit;

// namespace Test.Unit.FormerXunit
// {
//     public class FileMetadataParserTests
//     {
//         [Fact]
//         public void Test_FilemetadataParser_EmptyFileWithoutConfigOnlyGetsDefaultValues()
//         {
//             // Arange
//             MetadataParserOptions optionsMock = new MetadataParserOptions();
//             Mock<IMetadataProvider> metadataProviderMock = new Mock<IMetadataProvider>();

//             metadataProviderMock
//                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
//                 .Returns(new Metadata<FileMetaData> { });

//             LoggerMock<FileMetadataParser> loggerMock = new LoggerMock<FileMetadataParser>();
//             FileMetadataParser sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, optionsMock);
//             MetadataCriteria criteria = new MetadataCriteria
//             {
//                 Content = string.Empty,
//                 FileName = "file.html"
//             };

//             // Act
//             Metadata<FileMetaData> result = sut.Parse(criteria);

//             // Assert
//             result.Should().NotBeNull();
//             result.Data.Should().NotBeNull();
//             result.Data.Count.Should().Be(1, "Only URI is added by default");
//             result.Data.Uri.Should().NotBeNull();
//             result.Data.Uri.Should().Be("file.html");
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
//                         Values = new FileMetaData {}
//                     }
//                 }
//             };
//             Mock<IMetadataProvider> metadataProviderMock = new Mock<IMetadataProvider>();

//             metadataProviderMock
//                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
//                 .Returns(new Metadata<FileMetaData> { });

//             Mock<ILogger<FileMetadataParser>> loggerMock = new Mock<ILogger<FileMetadataParser>>();
//             FileMetadataParser sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
//             MetadataCriteria criteria = new MetadataCriteria
//             {
//                 Content = string.Empty,
//                 FileName = "file.html"
//             };

//             // Act
//             Metadata<FileMetaData> result = sut.Parse(criteria);

//             // Assert
//             result.Should().NotBeNull();
//             result.Data.Should().NotBeNull();
//             result.Data.Count.Should().Be(1, "Only URI is added by default");
//             result.Data.Uri.Should().NotBeNull();
//             result.Data.Uri.Should().Be("file.html");
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
//                         Values = new FileMetaData {
//                             Layout = "default.html"
//                         }
//                     }
//                 }
//             };
//             Mock<IMetadataProvider> metadataProviderMock = new Mock<IMetadataProvider>();

//             metadataProviderMock
//                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
//                 .Returns(new Metadata<FileMetaData> { });

//             Mock<ILogger<FileMetadataParser>> loggerMock = new Mock<ILogger<FileMetadataParser>>();
//             FileMetadataParser sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
//             MetadataCriteria criteria = new MetadataCriteria
//             {
//                 Content = string.Empty,
//                 FileName = "file.html"
//             };

//             // Act
//             Metadata<FileMetaData> result = sut.Parse(criteria);

//             // Assert
//             result.Should().NotBeNull();
//             result.Data.Should().NotBeNull();
//             result.Data.Count.Should().Be(2, "Defaults = 1 + Applied Config = 1, Makes 2 values");
//             result.Data.Uri.Should().NotBeNull();
//             result.Data.Uri.Should().Be("file.html");
//             result.Data.Layout.Should().NotBeNull();
//             result.Data.Layout.Should().Be("default.html");
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
//                         Values = new FileMetaData {
//                             Layout = "default.html"
//                         }
//                     },
//                     new DefaultMetadata {
//                         Path = "test",
//                         Values = new FileMetaData {
//                             Collection = "test"
//                         }
//                     }
//                 }
//             };
//             Mock<IMetadataProvider> metadataProviderMock = new Mock<IMetadataProvider>();

//             metadataProviderMock
//                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
//                 .Returns(new Metadata<FileMetaData>
//                 {
//                     Data = new FileMetaData
//                     {
//                         OutputLocation = "test/:name:ext"
//                     }
//                 });

//             Mock<ILogger<FileMetadataParser>> loggerMock = new Mock<ILogger<FileMetadataParser>>();
//             FileMetadataParser sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
//             MetadataCriteria criteria = new MetadataCriteria
//             {
//                 Content = string.Empty,
//                 FileName = "test/file.html"
//             };

//             // Act
//             Metadata<FileMetaData> result = sut.Parse(criteria);

//             // Assert
//             result.Should().NotBeNull();
//             result.Data.Should().NotBeNull();
//             result.Data.Count.Should().Be(3, "Defaults = 1 + Applied Config = 2, Makes 3 values");
//             result.Data.Uri.Should().NotBeNull();
//             result.Data.Uri.Should().Be("test/file.html");
//             result.Data.Layout.Should().NotBeNull();
//             result.Data.Layout.Should().Be("default.html");
//             result.Data.Collection.Should().NotBeNull();
//             result.Data.Collection.Should().Be("test");
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
//                         Values = new FileMetaData {
//                             Layout = "default.html"
//                         }
//                     },
//                     new DefaultMetadata {
//                         Path = "test",
//                         Values = new FileMetaData {
//                             Layout = "other.html",
//                             Collection = "test"
//                         }
//                     }
//                 }
//             };
//             Mock<IMetadataProvider> metadataProviderMock = new Mock<IMetadataProvider>();

//             metadataProviderMock
//                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
//                 .Returns(new Metadata<FileMetaData>
//                 {
//                     Data = new FileMetaData
//                     {
//                         OutputLocation = "test/:name:ext"
//                     }
//                 });

//             LoggerMock<FileMetadataParser> loggerMock = new LoggerMock<FileMetadataParser>();
//             FileMetadataParser sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
//             MetadataCriteria criteria = new MetadataCriteria
//             {
//                 Content = string.Empty,
//                 FileName = "test/file.html"
//             };

//             // Act
//             Metadata<FileMetaData> result = sut.Parse(criteria);

//             // Assert
//             result.Should().NotBeNull();
//             result.Data.Should().NotBeNull();
//             result.Data.Count.Should().Be(3, "Defaults = 1 + Applied Config = 2, Makes 3 values");
//             result.Data.Uri.Should().NotBeNull();
//             result.Data.Uri.Should().Be("test/file.html");
//             result.Data.Layout.Should().NotBeNull();
//             result.Data.Layout.Should().Be("other.html");
//             result.Data.Collection.Should().NotBeNull();
//             result.Data.Collection.Should().Be("test");
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
//                         Values = new FileMetaData {
//                             Layout = "default.html"
//                         }
//                     },
//                     new DefaultMetadata {
//                         Path = "test",
//                         Values = new FileMetaData {
//                             Collection = "test"
//                         }
//                     }
//                 }
//             };
//             Mock<IMetadataProvider> metadataProviderMock = new Mock<IMetadataProvider>();

//             metadataProviderMock
//                 .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
//                 .Returns(new Metadata<FileMetaData>
//                 {
//                     Data = new FileMetaData
//                     {
//                         OutputLocation = "posts/2021/:name:ext"
//                     }
//                 });

//             Mock<ILogger<FileMetadataParser>> loggerMock = new Mock<ILogger<FileMetadataParser>>();
//             FileMetadataParser sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
//             MetadataCriteria criteria = new MetadataCriteria
//             {
//                 Content = string.Empty,
//                 FileName = "posts/2021/file.html"
//             };

//             // Act
//             Metadata<FileMetaData> result = sut.Parse(criteria);

//             // Assert
//             result.Should().NotBeNull();
//             result.Data.Should().NotBeNull();
//             result.Data.Count.Should().Be(2, "Defaults = 1 + Applied Config = 1, Makes 2 values");
//             result.Data.Uri.Should().NotBeNull();
//             result.Data.Uri.Should().Be("posts/2021/file.html");
//             result.Data.Layout.Should().NotBeNull();
//             result.Data.Layout.Should().Be("default.html");
//         }
//     }
// }
