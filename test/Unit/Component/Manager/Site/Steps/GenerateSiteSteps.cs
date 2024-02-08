// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Kaylumah.Ssg.Manager.Site.Interface;
using Reqnroll;
using Reqnroll.Assist;
using Test.Unit.Entities;
using Test.Unit.Extensions;
using Test.Unit.Utilities;

namespace Test.Unit.Component.Manager.Site.Steps
{
    [Binding]
    [Scope(Feature = "SiteManager GenerateSite")]
    public class GenerateSiteSteps
    {
        readonly SiteManagerTestHarness _SiteManagerTestHarness;
        readonly ArticleCollection _ArticleCollection;
        readonly MockFileSystem _MockFileSystem;
        readonly ArtifactAccessMock _ArtifactAccess;

        public GenerateSiteSteps(
            ArtifactAccessMock artifactAccessMock,
            SiteManagerTestHarness siteManagerTestHarness,
            MockFileSystem mockFileSystem,
            ArticleCollection articleCollection)
        {
            _SiteManagerTestHarness = siteManagerTestHarness;
            _ArtifactAccess = artifactAccessMock;
            _MockFileSystem = mockFileSystem;
            _ArticleCollection = articleCollection;
        }

        [Given("the following articles:")]
        public void GivenTheFollowingArticles(ArticleCollection articleCollection)
        {
            // just an idea at the moment...
            // only the output dates are incorrect
            _ArticleCollection.AddRange(articleCollection);
            foreach (Article article in articleCollection)
            {
                Ssg.Extensions.Metadata.Abstractions.PageMetaData pageMeta = article.ToPageMetaData();
                MockFileData mockFile = MockFileDataFactory.EnrichedFile(string.Empty, pageMeta);
                string postFileName = Path.Combine(Constants.Directories.SourceDirectory, Constants.Directories.PostDirectory, article.Uri);
                _MockFileSystem.AddFile(postFileName, mockFile);
            }
        }

        [When("the site is generated:")]
        public async Task WhenTheSiteIsGenerated()
        {
            async Task Scenario(ISiteManager siteManager)
            {
                SiteConfiguration configuration = new SiteConfiguration()
                {
                    Source = Constants.Directories.SourceDirectory,
                    Destination = Constants.Directories.DestinationDirectory,
                    AssetDirectory = Constants.Directories.AssetDirectory,
                    DataDirectory = Constants.Directories.DataDirectory,
                    LayoutDirectory = Constants.Directories.LayoutDirectory,
                    PartialsDirectory = Constants.Directories.PartialsDirectory
                };
                GenerateSiteRequest generateSiteRequest = new GenerateSiteRequest(configuration);
                await siteManager.GenerateSite(generateSiteRequest);
            }

            await _SiteManagerTestHarness.TestSiteManager(Scenario).ConfigureAwait(false);
        }

        [Then("'(.*)' is a document with the following meta tags:")]
        public void ThenIsADocumentWithTheFollowingMetaTags(string documentPath, Table table)
        {
            ArgumentNullException.ThrowIfNull(table);
            System.Collections.Generic.List<(string Tag, string Value)> expected = table.CreateSet<(string Tag, string Value)>().ToList();
            HtmlAgilityPack.HtmlDocument html = _ArtifactAccess.GetHtmlDocument(documentPath);
            System.Collections.Generic.List<(string Tag, string Value)> actual = html.ToMetaTags();

            // Known issue: generator uses GitHash
            (string Tag, string Value) expectedGenerator = expected.Single(x => x.Tag == "generator");
            expected.Remove(expectedGenerator);
            (string Tag, string Value) actualGenerator = actual.Single(x => x.Tag == "generator");
            actual.Remove(actualGenerator);

            // Kaylumah tags
            actual.RemoveAll(x => x.Tag.StartsWith("kaylumah"));

            actual.Should().BeEquivalentTo(expected);
        }

        [Then("the following artifacts are created:")]
        public void ThenTheFollowingArtifactsAreCreated(Table table)
        {
            System.Collections.Generic.List<string> actualArtifacts = _ArtifactAccess
                .StoreArtifactRequests
                .SelectMany(x => x.Artifacts)
                .Select(x => x.Path)
                .ToList();

            string[] expectedArtifacts = table.Rows.Select(r => r[0]).ToArray();
            actualArtifacts.Should().BeEquivalentTo(expectedArtifacts);
        }
    }
}
