// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Engine.Transformation.Service;
using Kaylumah.Ssg.Manager.Site.Interface;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.Feed;
using Kaylumah.Ssg.Manager.Site.Service.Seo;
using Kaylumah.Ssg.Manager.Site.Service.SiteMap;
using Kaylumah.Ssg.Utilities.Time;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Test.Specflow.Entities;
using Test.Specflow.Utilities;

namespace Test.Specflow.Steps;

[Binding]
[Scope(Feature = "SiteManager")]
public class SiteManagerStepDefinitions
{
    private readonly ISiteManager _siteManager;
    private readonly ArticleCollection _articleCollection;
    private readonly ValidationContext _validationContext;
    private readonly ArtifactAccessMock _artifactAccess;
    // private readonly TransformationEngineMock _transformationEngine;

    public SiteManagerStepDefinitions(MockFileSystem mockFileSystem, ArticleCollection articleCollection, ValidationContext validationContext, SiteInfo siteInfo)
    {
        _articleCollection = articleCollection;
        _validationContext = validationContext;
        _artifactAccess = new ArtifactAccessMock();
        // _transformationEngine = new TransformationEngineMock();
        IMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(new YamlParser());
        ITransformationEngine transformationEngine = new TransformationEngine(
            NullLogger<TransformationEngine>.Instance,
            mockFileSystem,
            metadataProvider);
        var clock = new Mock<ISystemClock>();
        var fileProcessor = new FileProcessorMock(_articleCollection);
        var logger = NullLogger<SiteManager>.Instance;
        var yamlParser = new YamlParser();
        var siteMetadataFactory = new SiteMetadataFactory(clock.Object, siteInfo, yamlParser, mockFileSystem, NullLogger<SiteMetadataFactory>.Instance);
        var feedGenerator = new FeedGenerator(NullLogger<FeedGenerator>.Instance);
        var metaTagGenerator = new MetaTagGenerator(NullLogger<MetaTagGenerator>.Instance);
        var structureDataGenerator = new StructureDataGenerator(NullLogger<StructureDataGenerator>.Instance);
        var seoGenerator = new SeoGenerator(metaTagGenerator, structureDataGenerator);
        var siteMapGenerator = new SiteMapGenerator(NullLogger<SiteMapGenerator>.Instance);
        _siteManager = new SiteManager(
            fileProcessor.Object,
            _artifactAccess.Object,
            mockFileSystem,
            logger,
            siteInfo,
            transformationEngine,
            siteMetadataFactory,
            feedGenerator,
            seoGenerator,
            siteMapGenerator,
            clock.Object);
    }

    [When("the site is generated:")]
    public async Task WhenTheSiteIsGenerated()
    {
        try
        {
            await _siteManager.GenerateSite(new GenerateSiteRequest()
            {
                Configuration = new SiteConfiguration()
                {
                    Source = "_site",
                    AssetDirectory = "assets",
                    DataDirectory = "data",
                    LayoutDirectory = "_layouts",
                    PartialsDirectory = "_includes"
                }
            });
        }
        catch (Exception ex)
        {
            _validationContext.TestServiceException = ex;
        }
    }

    [Then("the atom feed artifacts has the following articles:")]
    public void ThenTheAtomFeedArtifactsHasTheFollowingArticles()
    {
        var feed = _artifactAccess.GetFeedArtifact();
        var articles = feed.ToArticles();
        
        var sitemap = _artifactAccess.GetSiteMapArtifact();

        var html = _artifactAccess.GetHtmlDocument("example.html");
        var htmlTags = html.ToMetaTags();
    }

    [Then("the following artifacts are created:")]
    public void ThenTheFollowingArtifactsAreCreated(Table table)
    {
        var actualArtifacts = _artifactAccess
            .StoreArtifactRequests
            .SelectMany(x => x.Artifacts)
            .Select(x => x.Path)
            .ToList();

        var expectedArtifacts = table.Rows.Select(r => r[0]).ToArray();
        actualArtifacts.Should().BeEquivalentTo(expectedArtifacts);
    }

    [Then("the scenario executed successfully:")]
    public void ThenTheScenarioExecutedSuccessfully()
    {
        _validationContext.TestServiceException.Should().BeNull();
    }
}
