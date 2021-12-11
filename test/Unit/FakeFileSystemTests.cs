// Copyright (c) Kaylumah, 2021. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Moq;
using Test.Utilities;
using Xunit;

namespace Test.Unit;

public class FakeFileSystemTests
{
    private readonly IFileProvider _fileProvider;
    private readonly string _rootDirectory;
    public FakeFileSystemTests()
    {
        _rootDirectory = "/a/b/c/";
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
            .SetupFileProviderMock(_rootDirectory, directories);
        _fileProvider = providerMock.Object;
    }

    [Fact]
    public void TestNonExistentDirectory()
    {
        // var nonExistentsAsDirectoryContents = fileProvider.GetDirectoryContents("other");
        // exists false
        // length 0
    }

    [Fact]
    public void TestNonExistentFile()
    {
        // var nonExistentsAsFileInfo = fileProvider.GetFileInfo("other.txt");
        // exist false, directory false // length throws
    }


    [Fact]
    public void Test1()
    {
        var rootDirectoryAsDirectoryContents = _fileProvider.GetDirectoryContents("");
        rootDirectoryAsDirectoryContents.Should().NotBeNull();
        rootDirectoryAsDirectoryContents.Exists.Should().BeTrue();
        rootDirectoryAsDirectoryContents.Count().Should().Be(2);

        var rootDirectoryAsFileInfo = _fileProvider.GetFileInfo("");
        // rootDirectoryAsFileInfo.Should().NotBeNull();
        // rootDirectoryAsFileInfo.Exists.Should().BeFalse();
        // rootDirectoryAsFileInfo.Name.Should().Be(string.Empty);
        // rootDirectoryAsFileInfo.PhysicalPath.Should().Be(null);
        // rootDirectoryAsFileInfo.Length.Should().Be(-1);
        // rootDirectoryAsFileInfo.IsDirectory.Should().BeFalse();

        var assetDirectoryAsDirectoryContents = _fileProvider.GetDirectoryContents("assets");
        assetDirectoryAsDirectoryContents.Should().NotBeNull();
        assetDirectoryAsDirectoryContents.Exists.Should().BeTrue();
        assetDirectoryAsDirectoryContents.Count().Should().Be(1);

        var assetDirectoryAsFileInfo = _fileProvider.GetFileInfo("assets");
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

        var indexHtmlAsFileInfo = _fileProvider.GetFileInfo("index.html");
        indexHtmlAsFileInfo.Should().NotBeNull();
        // exists true directory false
        // length, name, physical path
    }
}