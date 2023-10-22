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

namespace Test.Specflow.Component.Manager.Site.Steps
{
    [Binding]
    [Scope(Feature = "SiteManager GenerateSite")]
    public class GenerateSiteSteps
    {
        readonly SiteManagerTestHarness _siteManagerTestHarness;

        readonly ArticleCollection _articleCollection;
        readonly MockFileSystem _mockFileSystem;
        readonly ArtifactAccessMock _artifactAccess;

        public GenerateSiteSteps(
            ArtifactAccessMock artifactAccessMock,
            SiteManagerTestHarness siteManagerTestHarness,
            MockFileSystem mockFileSystem,
            ArticleCollection articleCollection)
        {
            _siteManagerTestHarness = siteManagerTestHarness;
            _artifactAccess = artifactAccessMock;
            _mockFileSystem = mockFileSystem;
            _articleCollection = articleCollection;
        }

        [Given("the following articles:")]
        public void GivenTheFollowingArticles(ArticleCollection articleCollection)
        {
            // just an idea at the moment...
            // only the output dates are incorrect
            _articleCollection.AddRange(articleCollection);
            foreach (Article article in articleCollection)
            {
                Ssg.Extensions.Metadata.Abstractions.PageMetaData pageMeta = article.ToPageMetaData();
                MockFileData mockFile = MockFileDataFactory.EnrichedFile(string.Empty, pageMeta);
                string postFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.PostDirectory, article.Uri);
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

        [Then("'(.*)' is a document with the following meta tags:")]
        public void ThenIsADocumentWithTheFollowingMetaTags(string documentPath, Table table)
        {
            ArgumentNullException.ThrowIfNull(table);
            System.Collections.Generic.List<(string Tag, string Value)> expected = table.CreateSet<(string Tag, string Value)>().ToList();
            HtmlAgilityPack.HtmlDocument html = _artifactAccess.GetHtmlDocument(documentPath);
            System.Collections.Generic.List<(string Tag, string Value)> actual = html.ToMetaTags();

            // Known issue: generator uses GitHash
            (string Tag, string Value) expectedGenerator = expected.Single(x => x.Tag == "generator");
            expected.Remove(expectedGenerator);
            (string Tag, string Value) actualGenerator = actual.Single(x => x.Tag == "generator");
            actual.Remove(actualGenerator);
            actual.Should().BeEquivalentTo(expected);
        }

        [Then("the following artifacts are created:")]
        public void ThenTheFollowingArtifactsAreCreated(Table table)
        {
            System.Collections.Generic.List<string> actualArtifacts = _artifactAccess
                .StoreArtifactRequests
                .SelectMany(x => x.Artifacts)
                .Select(x => x.Path)
                .ToList();

            string[] expectedArtifacts = table.Rows.Select(r => r[0]).ToArray();
            actualArtifacts.Should().BeEquivalentTo(expectedArtifacts);
        }
    }
}
