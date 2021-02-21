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
using System.Xml.Linq;

namespace Test.Unit
{
    public class FileProcessorTests
    {
        [Fact(Skip = "Not Completed")]
        public async Task Test1()
        {
            var root = "/a/b/c";
            var loggerMock = new Mock<ILogger<FileProcessor>>();
            var fileProviderMock = new Mock<IFileProvider>()
                .SetupFileProviderMock(
                    root,
                    new List<FakeDirectory>()
                    {
                        new FakeDirectory(string.Empty, new FakeFile[] {
                            new FakeFile("index.html", Encoding.UTF8.GetBytes("---\r\nlayout: 'default'---")),
                            new FakeFile("icon.png")
                        }),
                        new FakeDirectory("_posts", new FakeFile[] {
                            new FakeFile("_posts/post.md", Encoding.UTF8.GetBytes("---\r\nlayout: 'post'---"))
                        })
                    }
                );
            var fileSystem = new FileSystem(fileProviderMock.Object);
            var sut = new FileProcessor(fileSystem, loggerMock.Object, new IContentPreprocessorStrategy[] {});

            await sut.Process(null);
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