// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using Test.Specflow.Utilities;

namespace Test.Specflow.Steps;

[Binding]
public class FileSystemStepDefinitions
{
    private readonly MockFileSystem _mockFileSystem;
    private readonly string _postsDirectory = Path.Combine("_site", "_posts");
    private readonly string _pagesDirectory = Path.Combine("_site", "_pages");

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

    [Given("'(.*)' is a post with the following contents:")]
    public void GivenIsAPostWithTheFollowingContents(string fileName, string contents)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.PlainFile(contents));
    }
}
