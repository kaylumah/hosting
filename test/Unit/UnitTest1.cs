using System;
using Microsoft.Extensions.FileProviders;
using Moq;
using Xunit;
using Test.Utilities;
using System.Collections.Generic;
using static Test.Utilities.FileProviderExtensions;
using System.Linq;
using System.IO;

namespace Test.Unit
{
    public class UnitTest1
    {

        public void B(Mock<IFileProvider> fileProvider, FakeDirectory current, List<FakeDirectory> directories)
        {
            var subDirectories = directories.Where(x => x.FolderPath.Equals(current.Name)).ToList();
            directories = directories.Except(subDirectories).ToList();
            var fileInfos = new List<IFileInfo>();
            foreach(var file in current.Files)
            {
                var fileMock = new Mock<IFileInfo>();

                var relativePath = Path.Combine(current.Name, file.FilePath);
                fileMock.Setup(x => x.Name).Returns(file.FilePath);
                fileInfos.Add(fileMock.Object);
                fileProvider.Setup(x => x.GetFileInfo(It.Is<string>(p => p.Equals(relativePath))))
                    .Returns(fileMock.Object);
            }

            foreach(var directory in subDirectories)
            {
                var fileMock = new Mock<IFileInfo>();
                fileMock.Setup(x => x.Name).Returns(directory.Name);
                fileInfos.Add(fileMock.Object);
                B(fileProvider, directory, directories);
            }

            fileProvider
                .Setup(x => x.GetDirectoryContents(It.Is<string>(p => p.Equals(current.Name))))
                .Returns(fileInfos.CreateMock<IDirectoryContents, IFileInfo>().Object);
        }

        public void A (Mock<IFileProvider> fileProvider, List<FakeDirectory> directories)
        {
            var root = directories.SingleOrDefault(x => x.Name == string.Empty);
            directories.Remove(root);
            B(fileProvider, root, directories);
        }


        private List<object> Process(Mock<IFileProvider> fileProvider, List<FakeDirectory> directories, string basePath = "", int depth = 1)
        {
            var result = new List<object>();
            var targets = directories.Where(x => x.Depth == depth && x.FolderPath.Equals(basePath)).ToList();
            foreach(var target in targets)
            {
                directories.Remove(target);
            }
            foreach(var target in targets)
            {
                var subTargets = Process(fileProvider, directories, target.Name, depth + 1);
                


                // result.Add(new { Name = target.Name });
                result.AddRange(subTargets);
            }
            return result;
        }

        [Fact]
        public void Test2()
        {
            var provider = new Mock<IFileProvider>();
            var directories = new List<FakeDirectory>()
            {
                new FakeDirectory("", new FakeFile[] {
                    new FakeFile("index.html")
                }),
                new FakeDirectory("assets", new FakeFile[] {}),
                new FakeDirectory("assets/css", new FakeFile[] {
                    new FakeFile("styles.css")
                })
            };
            A(provider, directories);

            var build = provider.Object;

            var root = build.GetDirectoryContents(string.Empty);
            var assets = build.GetDirectoryContents("assets");
            var cssAssets = build.GetDirectoryContents("assets/css");

            var index = build.GetFileInfo("index.html");
            var css = build.GetFileInfo("assets/css/styles.css");
        }

        [Fact]
        public void Test1()
        {
            var provider = new Mock<IFileProvider>();
            var directories = new List<FakeDirectory>()
            {
                new FakeDirectory("assets", new FakeFile[] {}),
                new FakeDirectory("assets/css", new FakeFile[] {}),
                new FakeDirectory("assets/js", new FakeFile[] {}),
                new FakeDirectory("assets/images", new FakeFile[] {}),
                // new FakeDirectory("posts", new FakeFile[] {}),
                new FakeDirectory("posts/january", new FakeFile[] {})
            };
            var y = Process(provider, directories);
        }
    }
}
