using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Test.Utilities;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Microsoft.Extensions.Options;
using Kaylumah.Ssg.Manager.Site.Interface;
using FluentAssertions;
using System.Linq;

namespace Test.Unit
{
    public class FileProcessorTests
    {
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
                        })
                    }
                );
            var fileSystem = new FileSystem(fileProviderMock.Object);
            var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock);
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
            var optionsMock = Options.Create(new SiteInfo() {});
            var loggerMock = new Mock<ILogger<FileProcessor>>();
            var fileProviderMock = new Mock<IFileProvider>()
                .SetupFileProviderMock(
                    root,
                    new List<FakeDirectory>() {}
                );
            var fileSystem = new FileSystem(fileProviderMock.Object);
            var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock);
            var result = await sut.Process(new FileFilterCriteria {
                DirectoriesToSkip = new string[] {},
                FileExtensionsToTarget = new string[] {}
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
            var fileSystem = new FileSystem(fileProviderMock.Object);
            var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock);
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
            var fileSystem = new FileSystem(fileProviderMock.Object);
            var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] { }, optionsMock);
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
}