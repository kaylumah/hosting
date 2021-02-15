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

            var rootDirectoryDirectoryContents = fileProvider.GetDirectoryContents("");
            var rootDirectoryFileInfo = fileProvider.GetFileInfo("");

            var assets = fileProvider.GetDirectoryContents("assets");
            var cssAssets = fileProvider.GetDirectoryContents("assets/css");

            var index = fileProvider.GetFileInfo("index.html");
            var css = fileProvider.GetFileInfo("assets/css/styles.css");
        }
    }
}
