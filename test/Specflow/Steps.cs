// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

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

    [Given("something:")]
    public void Given()
    {
        _mockFileSystem.AddFile(Path.Combine("_site", "_posts", "demo.md"), new MockFileData(""));
        // _mockFileSystem.AddFile("date.md", new MockFileData(string.Empty));
        // _mockFileSystem.AddFile("other/one.md", new MockFileData(string.Empty));
        // _mockFileSystem.AddFile("other/nested/two.md", new MockFileData(string.Empty));
        // _mockFileSystem.AddFile("extra/nested/two.md", new MockFileData(string.Empty));
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
