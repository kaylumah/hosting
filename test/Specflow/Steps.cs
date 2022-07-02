// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Test.Specflow.Utilities;

namespace Test.Specflow;

using System.IO.Abstractions.TestingHelpers;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Utilities;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Microsoft.Extensions.Logging.Abstractions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Test.Specflow.Entities;
using Ssg.Extensions.Data.Yaml;
using FluentAssertions;

#pragma warning disable CS3001
[Binding]
public class Steps
{
    private readonly MockFileSystem _mockFileSystem = new();
    private readonly IFileSystem _fileSystem;
    private readonly IFileProcessor _fileProcessor;
    private readonly string _postsDirectory = Path.Combine("_site", "_posts");

    public Steps()
    {
        var metadataParser = new FileMetadataParser(NullLogger<FileMetadataParser>.Instance,
            new YamlFrontMatterMetadataProvider(new YamlParser()),
            new MetadataParserOptions());
        _fileSystem = new FileSystem(_mockFileSystem);
        _fileProcessor = new FileProcessor(_fileSystem,
            NullLogger<FileProcessor>.Instance,
            Enumerable.Empty<IContentPreprocessorStrategy>(),
            new SiteInfo(),
            metadataParser);
    }

    [Given("the following blog posts:")]
    [Given("the following blog articles:")]
    public void Given(Table table)
    {
        var posts = table.CreateSet<BlogPost>();
    }
    
    [Given("file '(.*)' has the following contents:")]
    public void GivenFileHasTheFollowingContents(string fileName, string contents)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.PlainFile(contents));
    }

    [When("the files are retrieved:")]
    public async Task When(Table table)
    {
        var (directoriesToSkip, targetExtensions) = table.CreateInstance<(string[] DirectoriesToSkip, string[] TargetExtensions)>();
        var criteria = new FileFilterCriteria()
        {
            DirectoriesToSkip = directoriesToSkip, FileExtensionsToTarget = targetExtensions
        };
        var result = await _fileProcessor.Process(criteria);
    }

    [Then("'(.*)' are valid")]
    public void Then(List<string> values)
    {
        var files = _mockFileSystem.AllFiles;
        files.Count().Should().Be(4);

        var directories = _mockFileSystem.AllDirectories;
        directories.Count().Should().Be(6, string.Join(",", _mockFileSystem.AllDirectories));
    }
}
#pragma warning restore CS3001
