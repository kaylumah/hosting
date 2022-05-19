// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
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


internal class MockFileDataFactory
{
    private string _frontMatter;
    private string _content;

    public static MockFileData DefaultFile(string content, Dictionary<string, object> data = null)
    {
        return new MockFileDataFactory()
            .WithYamlFrontMatter(data)
            .WithContent(content)
            .Create();
    }

    public static MockFileData DefaultFile(Dictionary<string, object> data = null)
    {
        return new MockFileDataFactory()
            .WithYamlFrontMatter(data)
            .WithContent(string.Empty)
            .Create();
    }

    public MockFileDataFactory WithYamlFrontMatter(Dictionary<string, object> data = null)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("---");
        if (data != null && data.Any())
        {
            var raw = new YamlDotNet.Serialization.Serializer().Serialize(data);
            stringBuilder.Append(raw);
        }
        stringBuilder.AppendLine("---");
        _frontMatter = stringBuilder.ToString();
        return this;
    }

    public MockFileDataFactory WithContent(string content)
    {
        _content = content;
        return this;
    }

    public MockFileData Create()
    {
        var stringBuilder = new StringBuilder();
        if (!string.IsNullOrEmpty(_frontMatter))
        {
            stringBuilder.Append(_frontMatter);
        }
        if (!string.IsNullOrEmpty(_content))
        {
            stringBuilder.Append(_content);
        }
        var fileData = stringBuilder.ToString();
        var bytes = Encoding.UTF8.GetBytes(fileData);
        return new MockFileData(bytes);
    }
}

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
        var data = MockFileDataFactory.DefaultFile("# This is Markdown", new Dictionary<string, object>
        {
            ["title"] = "Demo Post"
        });
        var text = Encoding.UTF8.GetString(data.Contents);

        var fileSystemData = new Dictionary<string, MockFileData>()
        {
            [Path.Combine("_site", "_posts", "example.md")] = data
        };

        Kaylumah.Ssg.Utilities.IFileSystem fileSystem = new Kaylumah.Ssg.Utilities.FileSystem(
                new MockFileSystem(fileSystemData)
        );
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

    [Given("file '(.*)' has the following contents:")]
    public void GivenFileHasTheFollowingContents(string fileName, string contents)
    {

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
            FileExtensionsToTarget = new string[]
            {
                ".md"
            }
        });
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
