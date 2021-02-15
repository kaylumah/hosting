using System;
using Microsoft.Extensions.FileProviders;
using Moq;
using Xunit;
using Test.Utilities;
using System.Collections.Generic;
using static Test.Utilities.FileProviderExtensions;
using System.Linq;
using System.IO;
using FluentAssertions;

namespace Test.Unit
{
    public class FakeFileSystemTests
    {
        [Fact]
        public void Test1()
        {
            var root = "/a/b/c/";
            var directories = new List<FakeDirectory>() {
                new FakeDirectory(string.Empty, new FakeFile[] {
                    new FakeFile("index.html")
                }),
                new FakeDirectory("assets", new FakeFile[] {}),
                new FakeDirectory("assets/css", new FakeFile[] {
                    new FakeFile("assets/css/styles.css")
                })
            };
            var providerMock = new Mock<IFileProvider>()
                .SetupFileProviderMock(root, directories);
            var fileProvider = providerMock.Object;

            var rootDirectoryAsDirectoryContents = fileProvider.GetDirectoryContents("");
            rootDirectoryAsDirectoryContents.Should().NotBeNull();
            rootDirectoryAsDirectoryContents.Exists.Should().BeTrue();
            rootDirectoryAsDirectoryContents.Count().Should().Be(2);

            var rootDirectoryAsFileInfo = fileProvider.GetFileInfo("");
            rootDirectoryAsFileInfo.Should().NotBeNull();
            // rootDirectoryAsFileInfo.Exists.Should().BeFalse();
            rootDirectoryAsFileInfo.Name.Should().Be(string.Empty);
            // rootDirectoryAsFileInfo.PhysicalPath.Should().Be(null);
            // rootDirectoryAsFileInfo.Length.Should().Be(-1);
            rootDirectoryAsFileInfo.IsDirectory.Should().BeFalse();

            var assetDirectoryAsDirectoryContents = fileProvider.GetDirectoryContents("assets");
            assetDirectoryAsDirectoryContents.Should().NotBeNull();
            assetDirectoryAsDirectoryContents.Exists.Should().BeTrue();
            assetDirectoryAsDirectoryContents.Count().Should().Be(1);

            var assetDirectoryAsFileInfo = fileProvider.GetFileInfo("assets");
            assetDirectoryAsFileInfo.Should().NotBeNull();
            // exists false
            // isdirectory false
            // length throws
            // name assets
            // physical path ....

            // var indexHtmlAsDirectoryContents = fileProvider.GetDirectoryContents("index.html");
            // indexHtmlAsDirectoryContents.Should().NotBeNull();
            // exists false
            // count = 0

            var indexHtmlAsFileInfo = fileProvider.GetFileInfo("index.html");
            indexHtmlAsFileInfo.Should().NotBeNull();
            // exists true directory false
            // length, name, physical path

            // var nonExistentsAsDirectoryContents = fileProvider.GetDirectoryContents("other");
            // exists false
            // length 0

            // var nonExistentsAsFileInfo = fileProvider.GetFileInfo("other.txt");
            // exist false, directory false // length throws
        }
    }
}
