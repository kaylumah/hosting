// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using Test.Specflow.Utilities;

namespace Test.Specflow.Steps;

[Binding]
public class FileSystemStepDefinitions
{
    private readonly MockFileSystem _mockFileSystem;
    private static readonly string _rootDirectory = "_site";
    private readonly string _postsDirectory = Path.Combine(_rootDirectory, "_posts");
    private readonly string _pagesDirectory = Path.Combine(_rootDirectory, "_pages");

    public FileSystemStepDefinitions(MockFileSystem mockFileSystem)
    {
        _mockFileSystem = mockFileSystem;
    }

    [Given("'(.*)' is an empty post:")]
    public void GivenIsAnEmptyPost(string fileName)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.EmptyFile());
    }

    [Given("'(.*)' is an empty page:")]
    public void GivenIsAnEmptyPage(string fileName)
    {
        var pageDirectory = Path.Combine(_pagesDirectory, fileName);
        _mockFileSystem.AddFile(pageDirectory, MockFileDataFactory.EmptyFile());
    }

    [Given("'(.*)' is an empty file:")]
    public void GivenIsAnEmptyFile(string fileName)
    {
        var normalizedFileName = fileName.Replace('/', Path.DirectorySeparatorChar);
        var filePath = Path.Combine(_rootDirectory, normalizedFileName);
        _mockFileSystem.AddFile(filePath, MockFileDataFactory.EmptyFile());
    }

    [Given("'(.*)' is a post with the following contents:")]
    public void GivenIsAPostWithTheFollowingContents(string fileName, string contents)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.PlainFile(contents));
    }
}
