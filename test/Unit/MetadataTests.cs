using System.IO;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Test.Unit
{
    public class MetadataTests
    {
        [Fact]
        public void Test_FileMetadataParser_EmptyInput()
        {
            var root = "/a/b/c";
            var fileName = "1.txt";
            var filePath = Path.Combine(root, "b/d/e/f", fileName);
            var criteria = new MetadataCriteria
            {
                FileName = fileName,
                FilePath = filePath,
                Root = root,
                Content = string.Empty
            };

            var loggerMock = new Mock<ILogger<FileMetadataParser>>();
            IFileMetadataParser sut = new Kaylumah.Ssg.Manager.Site.Service.FileMetadataParser(loggerMock.Object);
            var result = sut.Parse(criteria);
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public void Test_FileMetadataParser_EmptyInput2()
        {
            var root = "/a/b/c";
            var fileName = "1.txt";
            var filePath = Path.Combine(root, fileName);
            var criteria = new MetadataCriteria
            {
                FileName = fileName,
                FilePath = filePath,
                Root = root,
                Content = string.Empty
            };

            var loggerMock = new Mock<ILogger<FileMetadataParser>>();
            IFileMetadataParser sut = new FileMetadataParser(loggerMock.Object);
            var result = sut.Parse(criteria);
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
        }
    }
}