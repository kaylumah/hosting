// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO.Abstractions.TestingHelpers;
using System.Xml;
using FluentAssertions;
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
using Kaylumah.Ssg.Utilities.Time;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Ssg.Extensions.Data.Yaml;
using Ssg.Extensions.Metadata.Abstractions;
using Ssg.Extensions.Metadata.YamlFrontMatter;
using Test.Specflow.Entities;
using Test.Specflow.Extensions;
using Test.Specflow.Utilities;

namespace Test.Specflow.Steps;

[Binding]
[Scope(Feature = "SiteManager")]
public class SiteManagerStepDefinitions
{
    private readonly ScenarioContext _scenarioContext;
    private readonly ISiteManager _siteManager;
    private readonly ArticleCollection _articleCollection;
    private readonly ValidationContext _validationContext;
    private readonly MockFileSystem _mockFileSystem;
    private readonly SystemClockMock _systemClockMock;
    private readonly ArtifactAccessMock _artifactAccess;
    // private readonly TransformationEngineMock _transformationEngine;

    public SiteManagerStepDefinitions(SystemClockMock systemClockMock, ScenarioContext scenarioContext, MetadataParserOptions metadataParserOptions, MockFileSystem mockFileSystem, ArticleCollection articleCollection,
        ValidationContext validationContext, SiteInfo siteInfo)
    {
        _systemClockMock = systemClockMock;
        _scenarioContext = scenarioContext;
        _mockFileSystem = mockFileSystem;
        _articleCollection = articleCollection;
        _validationContext = validationContext;
        _artifactAccess = new ArtifactAccessMock();
        // _transformationEngine = new TransformationEngineMock();
        IMetadataProvider metadataProvider = new YamlFrontMatterMetadataProvider(new YamlParser());
        ITransformationEngine transformationEngine = new TransformationEngine(
            NullLogger<TransformationEngine>.Instance,
            mockFileSystem,
            metadataProvider);
        //var fileProcessor = new FileProcessorMock(_articleCollection);
        var metadataParser = new FileMetadataParser(NullLogger<FileMetadataParser>.Instance,
            new YamlFrontMatterMetadataProvider(new YamlParser()),
            metadataParserOptions);
        var fileProcessor = new FileProcessor(mockFileSystem,
            NullLogger<FileProcessor>.Instance,
            Enumerable.Empty<IContentPreprocessorStrategy>(),
            siteInfo,
            metadataParser);
        var logger = NullLogger<SiteManager>.Instance;
        var yamlParser = new YamlParser();
        var siteMetadataFactory = new SiteMetadataFactory(_systemClockMock.Object, siteInfo, yamlParser, mockFileSystem,
            NullLogger<SiteMetadataFactory>.Instance);
        var feedGenerator = new FeedGenerator(NullLogger<FeedGenerator>.Instance);
        var metaTagGenerator = new MetaTagGenerator(NullLogger<MetaTagGenerator>.Instance);
        var structureDataGenerator = new StructureDataGenerator(NullLogger<StructureDataGenerator>.Instance);
        var seoGenerator = new SeoGenerator(metaTagGenerator, structureDataGenerator);
        var siteMapGenerator = new SiteMapGenerator(NullLogger<SiteMapGenerator>.Instance);
        _siteManager = new SiteManager(
            // fileProcessor.Object,
            fileProcessor,
            _artifactAccess.Object,
            mockFileSystem,
            logger,
            siteInfo,
            transformationEngine,
            siteMetadataFactory,
            feedGenerator,
            seoGenerator,
            siteMapGenerator,
            _systemClockMock.Object);
    }

    [Given("the following articles:")]
    public void GivenTheFollowingArticles(ArticleCollection articleCollection)
    {
        // just an idea at the moment...
        // only the output dates are incorrect
        _articleCollection.AddRange(articleCollection);
        foreach (var article in articleCollection)
        {
            var pageMeta = article.ToPageMetaData();
            var mockFile = MockFileDataFactory.EnrichedFile(string.Empty, pageMeta);
            var postFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.PostDirectory, article.Uri);
            _mockFileSystem.AddFile(postFileName, mockFile);
        }
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
                    Source = Constants.Directories.SourceDirectory,
                    Destination = Constants.Directories.DestinationDirectory,
                    AssetDirectory = Constants.Directories.AssetDirectory,
                    DataDirectory = Constants.Directories.DataDirectory,
                    LayoutDirectory = Constants.Directories.LayoutDirectory,
                    PartialsDirectory = Constants.Directories.PartialsDirectory
                }
            });
        }
        catch (Exception ex)
        {
            _validationContext.TestServiceException = ex;
        }
    }

    [Then("the atom feed '(.*)' has the following articles:")]
    public async Task ThenTheAtomFeedArtifactHasTheFollowingArticles(string feedPath)
    {
        /*
        var feed = _artifactAccess.GetFeedArtifact(feedPath);
        await Verify(feed)
            .UseMethodName("AtomFeed");
        */
        var info = _scenarioContext.ScenarioInfo;
        var feed = _artifactAccess.GetString(feedPath);
        await Verify(feed)
            //.UseMethodName("AtomFeed")
            .AddScrubber(inputStringBuilder =>
            {
                var original = inputStringBuilder.ToString();
                using var reader = new StringReader(original);
                inputStringBuilder.Clear();

                var settings = new XmlReaderSettings();
                using var xmlReader = XmlReader.Create(new StringReader(original), settings);
                var document = new XmlDocument();  
                document.Load(reader);
                
                var manager = new XmlNamespaceManager(document.NameTable);
                manager.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                
                var valueElement = document.SelectSingleNode("//atom:feed/atom:updated", manager) as XmlElement;
                valueElement.InnerXml = "replaced";
                //inputStringBuilder.Append(document.OuterXml);
                
                XmlWriterSettings settings2 = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\r\n",
                    NewLineHandling = NewLineHandling.Replace
                };
                using var writer = XmlWriter.Create(inputStringBuilder, settings2);
                document.Save(writer);


                /*
                var navigator = document.CreateNavigator();
                if (navigator == null)
                {
                    return;
                }

                var manager = new XmlNamespaceManager(navigator.NameTable);
                manager.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                XPathNavigator x = navigator.SelectSingleNode("//atom:feed/atom:updated", manager);
                x?.SetValue("REPLACED_DATE");

                inputStringBuilder.Append(navigator.OuterXml);
                */
            });
    }

    [Then("the sitemap '(.*)' has the following articles:")]
    public async Task ThenTheSiteMapHasTheFollowingArticles(string sitemapPath)
    {
        /*
        var sitemap = _artifactAccess.GetSiteMapArtifact(sitemapPath);
        await Verify(sitemap)
            .UseMethodName("SiteMap");
            */
        await Task.CompletedTask;
    }

    [Then("'(.*)' is a document with the following meta tags:")]
    public async Task ThenIsADocumentWithTheFollowingMetaTags(string documentPath, Table table)
    {
        ArgumentNullException.ThrowIfNull(table);
        var expected = table.CreateSet<(string Tag, string Value)>().ToList();
        var html = _artifactAccess.GetHtmlDocument(documentPath);
        await Task.CompletedTask;
        await Verify(html.Text);
        var actual = html.ToMetaTags();

        // Known issue: generator uses GitHash
        var expectedGenerator = expected.Single(x => x.Tag == "generator");
        expected.Remove(expectedGenerator);
        var actualGenerator = actual.Single(x => x.Tag == "generator");
        actual.Remove(actualGenerator);
        actual.Should().BeEquivalentTo(expected);
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
