﻿// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Test.Specflow;

[Binding]
internal class FeatureSteps
{
    private readonly Dictionary<string, string> _extensionMapping = new();
    private readonly List<Page> _pages = new();
    private readonly List<string> _collections = new();
    private readonly List<string> _supportedFileExtensions = new();
    private readonly Dictionary<string, MockFileData> _fileSystemData = new();
    private Metadata<FileMetaData> _state;

    private IFileMetadataParser BuildFileMetadataParser()
    {
        var logger = NullLogger<FileMetadataParser>.Instance;
        var options = new MetadataParserOptions()
        { 
            ExtensionMapping = _extensionMapping
        };
        IYamlParser yamlParser = new YamlParser();
        IMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(yamlParser);
        var fileMetaDataParser = new FileMetadataParser(
            logger, metadataProvider, options);
        return fileMetaDataParser;
    }

    [Given("the following extensions:")]
    public void GivenTheFollowingExtensions(Table table)
    {
        // var test = table.CreateSet<(string key, string value)>();
        var test2 = table.ToDictionary();
        foreach(var item in test2)
        {
            _extensionMapping[item.Key] = (string)item.Value;
        }
    }

    private IFileProcessor BuildFileProcessor()
    {
        /*
        var data = MockFileDataFactory.DefaultFile("# This is Markdown", new Dictionary<string, object>
        {
            ["title"] = "Demo Post"
        });
        var text = Encoding.UTF8.GetString(data.Contents);

        
        var fileSystemData = new Dictionary<string, MockFileData>()
        {
            [Path.Combine("_site", "_posts", "example.md")] = data
        };
        */

        Kaylumah.Ssg.Utilities.IFileSystem fileSystem = new Kaylumah.Ssg.Utilities.FileSystem(
                new MockFileSystem(_fileSystemData)
        );
        var logger = NullLogger<FileProcessor>.Instance;
        var strategies = new IContentPreprocessorStrategy[]
        {
            new MarkdownContentPreprocessorStrategy()
        };
            
            //Array.Empty<IContentPreprocessorStrategy>();
        var siteInfo = new SiteInfo()
        {
            Collections = new Collections()
            {
                new Collection()
                {
                    Name = "posts",
                    Output = true
                }
            }
        };
        var metaDataParser = BuildFileMetadataParser();
        return new FileProcessor(fileSystem, logger, strategies, siteInfo, BuildFileMetadataParser());
    }

    [Then("the following pages:")]
    public void ThenTheFollowingPages(List<Page> pages)
    {
        _pages.Should().BeEquivalentTo(pages);
    }


    [Given("the extensions '(.*)' are targeted")]
    public void GivenTheFollowingExtensions(List<string> values)
    {
        _supportedFileExtensions.AddRange(values);
    }

    [Given("the collections '(.*)' are targeted")]
    public void GivenTheFollowingCollections(List<string> values)
    {
        _collections.AddRange(values);
    }

    [Given("scope '(.*)' has the following metadata:")]
    public void GivenTheFollowingData(string scope, Table table)
    {
        var metaData = table.ToDictionary();
    }

    [Given("file '(.*)' has the following contents:")]
    public void GivenFileHasTheFollowingContents(string fileName, string contents)
    {
        _fileSystemData.Add(fileName, MockFileDataFactory.DefaultFile(contents));
    }


    [When("something")]
    public void When()
    {
        /*
        var logger = NullLogger<FileMetadataParser>.Instance;
        var options = new MetadataParserOptions();
        var fileProvider = new Mock<IMetadataProvider>();
        fileProvider.Setup(mock => mock.Retrieve<FileMetaData>(It.IsAny<string>()))
            .Returns(new Metadata<FileMetaData> { });
        var fileMetaDataParser = new FileMetadataParser(
            logger, fileProvider.Object, options);
        */
        var fileMetaDataParser = BuildFileMetadataParser();

        var response = 
            fileMetaDataParser.Parse(
                new MetadataCriteria
                {
                    FileName = "1.txt",
                    Permalink = "2",
                    Content = String.Empty
                }
        );
        _state = response;
    }

    [When("something else")]
    public async Task WhenElse()
    {
        var fileProcessor = BuildFileProcessor();

        var result = await fileProcessor.Process(new FileFilterCriteria
        {
            FileExtensionsToTarget = _supportedFileExtensions.ToArray(),
            // DirectoriesToSkip
        });

        var pages = result.Select(x => new Page { 
            Uri = x.MetaData.Uri
        });
        _pages.AddRange(pages);
    }

    [Then("something")]
    public void Then(Table table)
    {
        var dict = table.ToDictionary();
        /*
        _state.Data
            .Should().Equal(dict);
        */
    }
}
