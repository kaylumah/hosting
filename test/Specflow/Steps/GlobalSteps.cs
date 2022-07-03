// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging.Abstractions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Test.Specflow.Entities;
using Test.Specflow.Extensions;
using Test.Specflow.Utilities;
using File = Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File;

namespace Test.Specflow.Steps;

[Binding]
public class GlobalSteps
{
    private readonly MockFileSystem _mockFileSystem = new();
    private readonly IFileProcessor _fileProcessor;
    private readonly string _postsDirectory = Path.Combine("_site", "_posts");
    private readonly List<File> _files = new();

    public GlobalSteps(MetadataParserOptions metadataParserOptions, SiteInfo siteInfo)
    {
        var metadataParser = new FileMetadataParser(NullLogger<FileMetadataParser>.Instance,
            new YamlFrontMatterMetadataProvider(new YamlParser()),
            metadataParserOptions);
        IFileSystem fileSystem = new FileSystem(_mockFileSystem);
        _fileProcessor = new FileProcessor(fileSystem,
            NullLogger<FileProcessor>.Instance,
            Enumerable.Empty<IContentPreprocessorStrategy>(),
            siteInfo,
            metadataParser);
    }

    [Given("'(.*)' is an empty post:")]
    public void GivenIsAnEmptyPost(string fileName)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.EmptyFile());
    }

    [Given("'(.*)' is a post with the following contents:")]
    public void GivenIsAPostWithTheFollowingContents(string fileName, string contents)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.PlainFile(contents));
    }

    [When("the files are retrieved:")]
    public async Task WhenTheFilesAreRetrieved(FileFilterCriteria criteria)
    {
        var result = await _fileProcessor.Process(criteria);
        _files.AddRange(result);
    }

    [Then("the following articles are returned:")]
    public void ThenTheFollowingArticlesAreReturned(ArticleCollection articleCollection)
    {
        var actual = _files.ToPages(Guid.NewGuid());
        var actualTransformed = actual.ToArticles();
        actualTransformed.Should().BeEquivalentTo(articleCollection);
    }
}
