// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Interface;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Test.Specflow.Entities;
using Test.Specflow.Extensions;
using Test.Specflow.Utilities;
using VerifyXunit;

namespace Test.Specflow.Component.Manager.Site.Steps;

[Binding]
[Scope(Feature = "SiteManager GenerateSite")]
public class GenerateSiteSteps
{
    private readonly SiteManagerTestHarness _siteManagerTestHarness;
    private readonly ScenarioContext _scenarioContext;
    private readonly ArticleCollection _articleCollection;
    private readonly MockFileSystem _mockFileSystem;
    private readonly ArtifactAccessMock _artifactAccess;

    public GenerateSiteSteps(
        ArtifactAccessMock artifactAccessMock,
        SiteManagerTestHarness siteManagerTestHarness,
        ScenarioContext scenarioContext, MockFileSystem mockFileSystem, ArticleCollection articleCollection)
    {
        _siteManagerTestHarness = siteManagerTestHarness;
        _artifactAccess = artifactAccessMock;
        _scenarioContext = scenarioContext;
        _mockFileSystem = mockFileSystem;
        _articleCollection = articleCollection;
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
        async Task Scenario(ISiteManager siteManager)
        {
            await siteManager.GenerateSite(new GenerateSiteRequest()
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

        await _siteManagerTestHarness.TestSiteManager(Scenario).ConfigureAwait(false);
    }

    [Then("the atom feed '(.*)' is verified:")]
    public async Task ThenTheAtomFeedIsVerified(string feedPath)
    {
        /*
        var feed = _artifactAccess.GetFeedArtifact(feedPath);
        await Verify(feed)
            .UseMethodName("AtomFeed");
        */
        var feed = _artifactAccess.GetString(feedPath);
        await Verifier.Verify(feed)
            .UseMethodName(_scenarioContext.ToVerifyMethodName("AtomFeed"))
            /*.AddScrubber(inputStringBuilder =>
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
            })*/
            ;
    }

    [Then("the sitemap '(.*)' is verified:")]
    public async Task ThenTheSitemapIsVerified(string sitemapPath)
    {
        /*
        var sitemap = _artifactAccess.GetSiteMapArtifact(sitemapPath);
        await Verify(sitemap)
            .UseMethodName("SiteMap");
            */
        var sitemap = _artifactAccess.GetString(sitemapPath);
        await Verifier.Verify(sitemap)
            .UseMethodName(_scenarioContext.ToVerifyMethodName("Sitemap"));

    }

    [Then("'(.*)' is a document with the following meta tags:")]
    public void ThenIsADocumentWithTheFollowingMetaTags(string documentPath, Table table)
    {
        ArgumentNullException.ThrowIfNull(table);
        var expected = table.CreateSet<(string Tag, string Value)>().ToList();
        var html = _artifactAccess.GetHtmlDocument(documentPath);
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
}
