// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Microsoft.Extensions.Logging.Abstractions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using TechTalk.SpecFlow;
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
        FileMetadataParser metadataParser = new FileMetadataParser(NullLogger<FileMetadataParser>.Instance,
            new YamlFrontMatterMetadataProvider(new YamlParser()),
            metadataParserOptions);
        _fileProcessor = new FileProcessor(mockFileSystem,
            NullLogger<FileProcessor>.Instance,
            Enumerable.Empty<IContentPreprocessorStrategy>(),
            siteInfo,
            metadataParser);
    }
    [When("the files are retrieved:")]
    public async Task WhenTheFilesAreRetrieved(FileFilterCriteria criteria)
    {
        IEnumerable<File> result = await _fileProcessor.Process(criteria);
        _files.AddRange(result);
    }

    [Then("the following articles are returned:")]
    public void ThenTheFollowingArticlesAreReturned(ArticleCollection articleCollection)
    {
        IEnumerable<Article> actual = _files.ToArticles();
        actual.Should().BeEquivalentTo(articleCollection);
    }

    [Then("no articles are returned:")]
    public void ThenNoArticlesAreReturned()
    {
        _files.Should().BeEmpty();
    }
}
