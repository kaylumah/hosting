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
using File = Kaylumah.Ssg.Manager.Site.Service.Files.Processor.File;

namespace Test.Specflow.Steps;

[Binding]
public class FileProcessorStepDefinitions
{
    private readonly IFileProcessor _fileProcessor;
    private readonly List<File> _files = new();

    public FileProcessorStepDefinitions(MockFileSystem mockFileSystem, MetadataParserOptions metadataParserOptions, SiteInfo siteInfo)
    {
        var metadataParser = new FileMetadataParser(NullLogger<FileMetadataParser>.Instance,
            new YamlFrontMatterMetadataProvider(new YamlParser()),
            metadataParserOptions);
        IFileSystem fileSystem = new FileSystem(mockFileSystem);
        _fileProcessor = new FileProcessor(fileSystem,
            NullLogger<FileProcessor>.Instance,
            Enumerable.Empty<IContentPreprocessorStrategy>(),
            siteInfo,
            metadataParser);
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
        var actual = _files.ToArticles();
        actual.Should().BeEquivalentTo(articleCollection);
    }

    [Then("no articles are returned:")]
    public void ThenNoArticlesAreReturned()
    {
        _files.Should().BeEmpty();
    }
}
