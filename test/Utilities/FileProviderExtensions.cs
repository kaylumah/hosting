using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Moq;

namespace Test.Utilities
{

    public static class FileProviderExtensions
    {
        public static Mock<IFileProvider> SetupFileProviderMock(this Mock<IFileProvider> fileProviderMock, string root, List<FakeDirectory> directories)
        {
            var rootDirectory = directories.SingleOrDefault(x => x.Name == string.Empty);
            if (rootDirectory != null)
            {
                directories.Remove(rootDirectory);
            }
            ProcessDirectory(fileProviderMock, root, rootDirectory, directories);
            return fileProviderMock;
        }

        public static IFileInfo CreateDirectory(this Mock<IFileProvider> fileProviderMock, string root, string folderPath)
        {
            var directoryMock = new Mock<IFileInfo>();
            var directoryName = folderPath.Split(Path.DirectorySeparatorChar).Last();
            directoryMock.Setup(x => x.IsDirectory).Returns(true);
            directoryMock.Setup(x => x.Length).Throws(new FileNotFoundException());
            directoryMock.Setup(x => x.CreateReadStream()).Throws(new UnauthorizedAccessException());
            directoryMock.Setup(x => x.Exists).Returns(true);
            directoryMock.Setup(x => x.PhysicalPath).Returns(Path.Combine(root, folderPath));
            directoryMock.Setup(x => x.Name).Returns(directoryName);

            var directory = directoryMock.Object;
            fileProviderMock.Setup(x => x.GetFileInfo(It.Is<string>(p => p.Equals(folderPath)))).Returns(directory);
            return directory;
        }

        public static IFileInfo CreateFile(this Mock<IFileProvider> fileProviderMock, string root, string filePath, byte[] bytes)
        {
            var fileMock = new Mock<IFileInfo>();
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
            fileMock.Setup(x => x.IsDirectory).Returns(false);
            fileMock.Setup(x => x.Length).Returns(() => new MemoryStream(bytes).Length);
            fileMock.Setup(x => x.CreateReadStream()).Returns(() => new MemoryStream(bytes));
            fileMock.Setup(x => x.Exists).Returns(true);
            fileMock.Setup(x => x.PhysicalPath).Returns(Path.Combine(root, filePath));
            fileMock.Setup(x => x.Name).Returns(fileName);

            var file = fileMock.Object;
            fileProviderMock.Setup(x => x.GetFileInfo(It.Is<string>(p => p.Equals(filePath)))).Returns(file);
            return file;
        }

        public static Mock<IFileProvider> AddFile(this Mock<IFileProvider> fileProviderMock, string root, string filePath, byte[] bytes)
        {
            fileProviderMock.CreateFile(root, filePath, bytes);
            return fileProviderMock;
        }
        public static Mock<IFileProvider> AddDirectory(this Mock<IFileProvider> fileProviderMock, string root, string folderPath)
        {
            fileProviderMock.CreateDirectory(root, folderPath);
            return fileProviderMock;
        }

        private static void ProcessDirectory(Mock<IFileProvider> fileProviderMock, string root, FakeDirectory current, List<FakeDirectory> directories)
        {
            var subDirectories = directories.Where(x => x.FolderPath.Equals(current.Name)).ToList();
            directories = directories.Except(subDirectories).ToList();
            var fileInfos = new List<IFileInfo>();
            foreach (var file in current.Files)
            {
                var fileInfo = fileProviderMock.CreateFile(root, file.FilePath, file.Bytes);
                fileInfos.Add(fileInfo);
            }
            foreach (var directory in subDirectories)
            {
                var directoryInfo = fileProviderMock.CreateDirectory(root, directory.Name);
                fileInfos.Add(directoryInfo);
                ProcessDirectory(fileProviderMock, root, directory, directories);
            }

            var directoryContents = fileInfos.CreateMock<IDirectoryContents, IFileInfo>();
            directoryContents.Setup(x => x.Exists).Returns(true);
            fileProviderMock
                .Setup(x => x.GetDirectoryContents(It.Is<string>(p => p.Equals(current.Name))))
                .Returns(directoryContents.Object);
        }
    }

    public class FakeFile
    {
        public string FilePath { get; }
        public byte[] Bytes { get; }
        public FakeFile(string relativePath) : this(relativePath, Encoding.UTF8.GetBytes(string.Empty))
        {
        }

        public FakeFile(string relativePath, byte[] bytes)
        {
            FilePath = relativePath;
            Bytes = bytes;
        }
    }

    public class FakeDirectory
    {
        public string FolderPath { get; set; }
        public string Name { get; }
        public int Depth { get; }
        public FakeFile[] Files { get; }

        public FakeDirectory(string name, FakeFile[] files)
        {
            Name = name;
            Depth = Name.Split(Path.DirectorySeparatorChar).Count();
            var index = Name.LastIndexOf(Path.DirectorySeparatorChar);
            if (index > 0)
            {
                FolderPath = name.Substring(0, index);
            }
            else
            {
                FolderPath = string.Empty;
            }

            Files = files;
        }
    }

    public static class EnumerableExtensions
    {
        public static Mock<TCollection> CreateMock<TCollection, TItem> (this IEnumerable<TItem> collection)
            where TCollection : class, IEnumerable<TItem>, IEnumerable
            where TItem : class
            {
                var mock = new Mock<TCollection>();
                // mock.Setup(x => x.Count()).Returns(() => collection.Count());
                // mock.Setup(m => m[It.IsAny<int>()]).Returns<int>(i => items.ElementAt(i));
                mock.Setup(m => m.GetEnumerator()).Returns(() => collection.GetEnumerator());
                return mock;
            }
    }
}
