// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ssg.Extensions.Metadata.Abstractions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Test.Specflow;

[Binding]
internal class FeatureSteps
{
    [Given("scope '(.*)' has the following metadata:")]
    public void GivenTheFollowingData(string scope, Table table)
    {
        var metaData = table.ToDictionary();
    }

    [When("something")]
    public void When()
    {
        var logger = NullLogger<FileMetadataParser>.Instance;
        var options = new MetadataParserOptions();
        var fileProvider = new Mock<IMetadataProvider>();
        fileProvider.Setup(mock => mock.Retrieve<FileMetaData>(It.IsAny<string>()))
            .Returns(new Metadata<FileMetaData> { });
        var fileMetaDataParser = new FileMetadataParser(
            logger, fileProvider.Object, options);

        var response = 
            fileMetaDataParser.Parse(
                new MetadataCriteria
                {
                    FileName = "1.txt",
                    Permalink = "2"
                }
        );
    }
}
