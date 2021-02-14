using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Moq;

namespace Test.Utilities
{
    public static class FileProviderExtensions
    {
        public static Mock<IFileProvider> AddFakeDirectory(this Mock<IFileProvider> fileProviderMock, string root, string folderPath)
        {
            var directoryMock = new Mock<IFileInfo>();
            var directoryName = folderPath.Split(Path.DirectorySeparatorChar).Last();
            directoryMock.Setup(x => x.IsDirectory).Returns(false);
            directoryMock.Setup(x => x.Length).Throws(new FileNotFoundException());
            directoryMock.Setup(x => x.CreateReadStream()).Throws(new UnauthorizedAccessException());
            directoryMock.Setup(x => x.Exists).Returns(true);
            directoryMock.Setup(x => x.PhysicalPath).Returns(Path.Combine(root, folderPath));
            directoryMock.Setup(x => x.Name).Returns(directoryName);

            fileProviderMock.Setup(x => x.GetFileInfo(It.Is<string>(p => p.Equals(folderPath)))).Returns(
                directoryMock.Object);
            return fileProviderMock;
        }

        public static Mock<IFileProvider> AddFakeFile(this Mock<IFileProvider> fileProviderMock, string root, string filePath, byte[] bytes)
        {
            var fileMock = new Mock<IFileInfo>();
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
            fileMock.Setup(x => x.IsDirectory).Returns(false);
            fileMock.Setup(x => x.Length).Returns(() => new MemoryStream(bytes).Length);
            fileMock.Setup(x => x.CreateReadStream()).Returns(() => new MemoryStream(bytes));
            fileMock.Setup(x => x.Exists).Returns(true);
            fileMock.Setup(x => x.PhysicalPath).Returns(Path.Combine(root, filePath));
            fileMock.Setup(x => x.Name).Returns(fileName);

            fileProviderMock.Setup(x => x.GetFileInfo(It.Is<string>(p => p.Equals(filePath)))).Returns(
                fileMock.Object);
            return fileProviderMock;
        }
    }
}
