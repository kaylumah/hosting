// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
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
    private readonly IFileProcessor _fileProcessor;
    private readonly string _postsDirectory = Path.Combine("_site", "_posts");
    private readonly List<File> _files = new();

    public Steps()
    {
        var metadataParser = new FileMetadataParser(NullLogger<FileMetadataParser>.Instance,
            new YamlFrontMatterMetadataProvider(new YamlParser()),
            new MetadataParserOptions()
            {
                
            });
        IFileSystem fileSystem = new FileSystem(_mockFileSystem);
        _fileProcessor = new FileProcessor(fileSystem,
            NullLogger<FileProcessor>.Instance,
            Enumerable.Empty<IContentPreprocessorStrategy>(),
            new SiteInfo()
            {
                Collections = new Collections()
                {
                    new Collection()
                    {
                        Name = "posts",
                        Output = true
                    }
                }
            },
            metadataParser);
    }

    
    
    [Given("post '(.*)' has the following contents:")]
    public void GivenFileHasTheFollowingContents(string fileName, string contents)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.PlainFile(contents));
    }

    [Given("a test post named '(.*)':")]
    public void GivenATestPost(string fileName)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.EnrichedFile("Hello World!"));
    }
    
    [Given("a test post v2 named '(.*)':")]
    public void GivenATestPostV2(string fileName)
    {
        var articleDirectory = Path.Combine(_postsDirectory, fileName);
        _mockFileSystem.AddFile(articleDirectory, MockFileDataFactory.EnrichedFile("Hello World!", new Dictionary<string, object>()
        {
            ["output"] = true
        }));
    }

    [When("the files are retrieved:")]
    public async Task WhenTheFilesAreRetrieved(FileFilterCriteria criteria)
    {
        var result = await _fileProcessor.Process(criteria);
        _files.AddRange(result);
    }

    [Then("the following:")]
    public void ThenTheFollowing(Table table)
    {
        var actual = new List<(string Path, string Key, object Value)>();
        foreach (var file in _files)
        {
            foreach (var item in file.MetaData)
            {
                actual.Add(new(file.Name, item.Key, item.Value));
            }
        }

        var expected = table
            .CreateSet<(string Path, string Key, object Value)>()
            .ToList();

        actual.Should().BeEquivalentTo(expected);
    }

    [StepArgumentTransformation]
    private static FileFilterCriteria ToFileFilterCriteria(Table table)
    {
        var criteria = table.CreateInstance<FileFilterCriteria>();
        return criteria;
    }
}
#pragma warning restore CS3001
