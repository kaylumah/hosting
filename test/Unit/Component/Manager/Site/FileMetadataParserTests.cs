// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Microsoft.Extensions.Logging;
using Moq;
using Ssg.Extensions.Metadata.Abstractions;
using Test.Unit.Mocks;
using Xunit;

namespace Test.Unit;

public class FileMetadataParserTests
{
    [Fact]
    public void Test_FilemetadataParser_EmptyFileWithoutConfigOnlyGetsDefaultValues()
    {
        // Arange
        var optionsMock = new MetadataParserOptions();
        var metadataProviderMock = new Mock<IMetadataProvider>();

        metadataProviderMock
            .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
            .Returns(new Metadata<FileMetaData> { 
                Data = new FileMetaData()
            });

        var loggerMock = new LoggerMock<FileMetadataParser>();
        var sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, optionsMock);
        var criteria = new MetadataCriteria
        {
            Content = string.Empty,
            FileName = "file.html"
        };

        // Act
        var result = sut.Parse(criteria);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count.Should().Be(1, "Only URI is added by default");
        result.Data.Uri.Should().NotBeNull();
        result.Data.Uri.Should().Be("file.html");
    }

    [Fact]
    public void Test_FilemetadataParser_EmptyFileWithConfigThatIsEmptyOnlyGetsDefaultValues()
    {
        // Arange
        var options = new MetadataParserOptions
        {
            Defaults = new DefaultMetadatas {
                    new DefaultMetadata {
                        Path = string.Empty,
                        Values = new FileMetaData {}
                    }
                }
        };
        var metadataProviderMock = new Mock<IMetadataProvider>();

        metadataProviderMock
            .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
            .Returns(new Metadata<FileMetaData> { 
                Data = new FileMetaData()
            });

        var loggerMock = new Mock<ILogger<FileMetadataParser>>();
        var sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
        var criteria = new MetadataCriteria
        {
            Content = string.Empty,
            FileName = "file.html"
        };

        // Act
        var result = sut.Parse(criteria);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count.Should().Be(1, "Only URI is added by default");
        result.Data.Uri.Should().NotBeNull();
        result.Data.Uri.Should().Be("file.html");
    }

    [Fact]
    public void Test_FilemetadataParser_EmptyFileWithConfigTGetsDefaultValuesAndConfiguration()
    {
        // Arange
        var options = new MetadataParserOptions
        {
            Defaults = new DefaultMetadatas {
                    new DefaultMetadata {
                        Path = string.Empty,
                        Values = new FileMetaData {
                            Layout = "default.html"
                        }
                    }
                }
        };
        var metadataProviderMock = new Mock<IMetadataProvider>();

        metadataProviderMock
            .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
            .Returns(new Metadata<FileMetaData> { 
                Data = new FileMetaData()
            });

        var loggerMock = new Mock<ILogger<FileMetadataParser>>();
        var sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
        var criteria = new MetadataCriteria
        {
            Content = string.Empty,
            FileName = "file.html"
        };

        // Act
        var result = sut.Parse(criteria);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count.Should().Be(2, "Defaults = 1 + Applied Config = 1, Makes 2 values");
        result.Data.Uri.Should().NotBeNull();
        result.Data.Uri.Should().Be("file.html");
        result.Data.Layout.Should().NotBeNull();
        result.Data.Layout.Should().Be("default.html");
    }

    [Fact]
    public void Test_FilemetadataParser_EmptyFileWithConfigTGetsDefaultValuesAndMultipleConfigurations()
    {
        // Arange
        var options = new MetadataParserOptions
        {
            Defaults = new DefaultMetadatas {
                    new DefaultMetadata {
                        Path = string.Empty,
                        Values = new FileMetaData {
                            Layout = "default.html"
                        }
                    },
                    new DefaultMetadata {
                        Path = "test",
                        Values = new FileMetaData {
                            Collection = "test"
                        }
                    }
                }
        };
        var metadataProviderMock = new Mock<IMetadataProvider>();

        metadataProviderMock
            .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
            .Returns(new Metadata<FileMetaData> { 
                Data = new FileMetaData {
                    OutputLocation = "test/:name:ext"
                }
            });

        var loggerMock = new Mock<ILogger<FileMetadataParser>>();
        var sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
        var criteria = new MetadataCriteria
        {
            Content = string.Empty,
            FileName = "test/file.html"
        };

        // Act
        var result = sut.Parse(criteria);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count.Should().Be(3, "Defaults = 1 + Applied Config = 2, Makes 3 values");
        result.Data.Uri.Should().NotBeNull();
        result.Data.Uri.Should().Be("test/file.html");
        result.Data.Layout.Should().NotBeNull();
        result.Data.Layout.Should().Be("default.html");
        result.Data.Collection.Should().NotBeNull();
        result.Data.Collection.Should().Be("test");
    }

    [Fact]
    public void Test_FilemetadataParser_EmptyFileIfMultipleConfigurationsApplyLastOneWins()
    {
        // Arange
        var options = new MetadataParserOptions
        {
            Defaults = new DefaultMetadatas {
                    new DefaultMetadata {
                        Path = string.Empty,
                        Values = new FileMetaData {
                            Layout = "default.html"
                        }
                    },
                    new DefaultMetadata {
                        Path = "test",
                        Values = new FileMetaData {
                            Layout = "other.html",
                            Collection = "test"
                        }
                    }
                }
        };
        var metadataProviderMock = new Mock<IMetadataProvider>();

        metadataProviderMock
            .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
            .Returns(new Metadata<FileMetaData> {
                Data = new FileMetaData {
                    OutputLocation = "test/:name:ext"
                }
             });

        var loggerMock = new LoggerMock<FileMetadataParser>();
        var sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
        var criteria = new MetadataCriteria
        {
            Content = string.Empty,
            FileName = "test/file.html"
        };

        // Act
        var result = sut.Parse(criteria);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count.Should().Be(3, "Defaults = 1 + Applied Config = 2, Makes 3 values");
        result.Data.Uri.Should().NotBeNull();
        result.Data.Uri.Should().Be("test/file.html");
        result.Data.Layout.Should().NotBeNull();
        result.Data.Layout.Should().Be("other.html");
        result.Data.Collection.Should().NotBeNull();
        result.Data.Collection.Should().Be("test");
    }

    [Fact]
    public void Test_FilemetadataParser_MultipleLayers()
    {
        // Arange
        var options = new MetadataParserOptions
        {
            Defaults = new DefaultMetadatas {
                    new DefaultMetadata {
                        Path = string.Empty,
                        Values = new FileMetaData {
                            Layout = "default.html"
                        }
                    },
                    new DefaultMetadata {
                        Path = "test",
                        Values = new FileMetaData {
                            Collection = "test"
                        }
                    }
                }
        };
        var metadataProviderMock = new Mock<IMetadataProvider>();

        metadataProviderMock
            .Setup(x => x.Retrieve<FileMetaData>(It.Is<string>(p => p.Equals(string.Empty))))
            .Returns(new Metadata<FileMetaData> { 
                Data = new FileMetaData {
                    OutputLocation = "posts/2021/:name:ext"
                }
            });

        var loggerMock = new Mock<ILogger<FileMetadataParser>>();
        var sut = new FileMetadataParser(loggerMock.Object, metadataProviderMock.Object, options);
        var criteria = new MetadataCriteria
        {
            Content = string.Empty,
            FileName = "posts/2021/file.html"
        };

        // Act
        var result = sut.Parse(criteria);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Count.Should().Be(2, "Defaults = 1 + Applied Config = 1, Makes 2 values");
        result.Data.Uri.Should().NotBeNull();
        result.Data.Uri.Should().Be("posts/2021/file.html");
        result.Data.Layout.Should().NotBeNull();
        result.Data.Layout.Should().Be("default.html");
    }
}
