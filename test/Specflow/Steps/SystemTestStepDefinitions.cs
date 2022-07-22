// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using Kaylumah.Ssg.Access.Artifact.Interface;
using Kaylumah.Ssg.Access.Artifact.Service;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Files.Metadata;
using Kaylumah.Ssg.Manager.Site.Service.Files.Preprocessor;
using Kaylumah.Ssg.Manager.Site.Service.Files.Processor;
using Kaylumah.Ssg.Manager.Site.Service.Seo;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Kaylumah.Ssg.Utilities;
using Kaylumah.Ssg.Utilities.Time;
using Microsoft.Extensions.Logging.Abstractions;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Test.Specflow.Entities;
using Test.Specflow.Extensions;
using Test.Specflow.Utilities;

namespace Test.Specflow.Steps;

[Binding]
public class SystemTestStepDefinitions
{
    private readonly ISiteManager _siteManager;
    private readonly MockFileSystem _mockFileSystem;
    
    public SystemTestStepDefinitions(MockFileSystem mockFileSystem, SiteInfo siteInfo,
        MetadataParserOptions metadataParserOptions)
    {
        _mockFileSystem = mockFileSystem;
        List<IStoreArtifactsStrategy> strategies = new List<IStoreArtifactsStrategy>()
        {
            new FileSystemStoreArtifactsStrategy(NullLogger<FileSystemStoreArtifactsStrategy>.Instance,
                mockFileSystem)
        };

        IArtifactAccess artifactAccess = new ArtifactAccess(NullLogger<ArtifactAccess>.Instance, strategies);

        IYamlParser yamlParser = new YamlParser();

        IMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(yamlParser);

        ITransformationEngine transformationEngine = new TransformationEngine(NullLogger<TransformationEngine>.Instance,
            mockFileSystem, metadataProvider);

        IFileMetadataParser fileMetadataParser = new FileMetadataParser(NullLogger<FileMetadataParser>.Instance,
            metadataProvider, metadataParserOptions);

        IFileProcessor fileProcessor = new FileProcessor(
            mockFileSystem,
            NullLogger<FileProcessor>.Instance,
            Enumerable.Empty<IContentPreprocessorStrategy>(),
            siteInfo,
            fileMetadataParser);

        ISystemClock clock = new SystemClock();

        var factory = new SiteMetadataFactory(clock,
            siteInfo,
            yamlParser,
            mockFileSystem,
            NullLogger<SiteMetadataFactory>.Instance);

        _siteManager = new SiteManager(
            fileProcessor,
            artifactAccess,
            mockFileSystem,
            NullLogger<SiteManager>.Instance,
            siteInfo,
            transformationEngine,
            factory,
            new FeedGenerator(NullLogger<FeedGenerator>.Instance),
            new SeoGenerator(new MetaTagGenerator(NullLogger<MetaTagGenerator>.Instance), new StructureDataGenerator(NullLogger<StructureDataGenerator>.Instance)),
            new SiteMapGenerator(NullLogger<SiteMapGenerator>.Instance),
            clock
        );
    }

    [Given("the following articles v2:")]
    public void GivenTheFollowingArticles(ArticleCollection articleCollection)
    {
        var files = articleCollection.ToPageMetaData().ToFile().ToList();
        foreach (var file in files)
        {
            var mockFile = MockFileDataFactory.EnrichedFile(file.Content, file.MetaData);
            _mockFileSystem.AddFile(Path.Combine(Constants.SourceDirectory, Constants.PostDirectory, file.Name), mockFile);
        }
    }

    [When("the site is generated v2:")]
    public async Task WhenTheSiteIsGenerated()
    {
        try
        {
            await _siteManager.GenerateSite(new GenerateSiteRequest()
            {
                Configuration = new SiteConfiguration()
                {
                    Source = Constants.SourceDirectory,
                    Destination = Constants.DestinationDirectory,
                    AssetDirectory = Constants.AssetDirectory,
                    DataDirectory = Constants.DataDirectory,
                    LayoutDirectory = Constants.LayoutDirectory,
                    PartialsDirectory = Constants.PartialsDirectory
                }
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    [Then("the following:")]
    public void ThenFollowing()
    {
        var fileNames = _mockFileSystem.AllFiles.ToList();
        var bytes = _mockFileSystem.GetFileBytes("dist/feed.xml");
    }
}
