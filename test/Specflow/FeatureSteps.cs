// Copyright (c) Kaylumah, 2022. All rights reserved.
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

    private IFileMetadataParser BuildFileMetadataParser()
    {
        var logger = NullLogger<FileMetadataParser>.Instance;
        var options = new MetadataParserOptions();
        IYamlParser yamlParser = new YamlParser();
        IMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(yamlParser);
        var fileMetaDataParser = new FileMetadataParser(
            logger, metadataProvider, options);
        return fileMetaDataParser;
    }

    private IFileProcessor BuildFileProcessor()
    {
        Kaylumah.Ssg.Utilities.IFileSystem fileSystem = new Kaylumah.Ssg.Utilities.FileSystem(new MockFileSystem());
        var logger = NullLogger<FileProcessor>.Instance;
        var strategies = Array.Empty<IContentPreprocessorStrategy>();
        var siteInfo = new SiteInfo();
        var metaDataParser = BuildFileMetadataParser();
        return new FileProcessor(fileSystem, logger, strategies, siteInfo, BuildFileMetadataParser());
    }



    private Metadata<FileMetaData> _state;

    [Given("scope '(.*)' has the following metadata:")]
    public void GivenTheFollowingData(string scope, Table table)
    {
        var metaData = table.ToDictionary();
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

        var result = await fileProcessor.Process(new FileFilterCriteria { });
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

public class Custom : Dictionary<string, object>
{
}
