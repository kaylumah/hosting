// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Service;
using Ssg.Extensions.Metadata.Abstractions;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Test.Unit
{
    public class SiteMetadataSnapshotTests
    {
        readonly VerifySettings _VerifySettings;
        const string DefaultSiteId = "my-site";
        const string DefaultPageId = "my-page";

        public SiteMetadataSnapshotTests()
        {
            _VerifySettings = new VerifySettings();
        }

        [Fact]
        public async Task Test_EmptySite_ResultsInDefaults()
        {
            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));

            List<BasePage> items = new();
            Dictionary<string, object> data = new();
            SiteMetaData siteMetaData = new SiteMetaData(DefaultSiteId, "", "", "", "", "", data, buildData, items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_OnlyTags()
        {
            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));

            TagMetaDataCollection tagMetaDataCollection = new();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = "1";
            tagMetaDataCollection.Add(tagMetaData);
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };

            List<BasePage> items = new();
            SiteMetaData siteMetaData = new SiteMetaData(DefaultSiteId, "", "", "", "", "", data, buildData, items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_OnlyPages()
        {
            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));

            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                { "baseuri", "http://127.0.0.1" },
                { "uri", "example.html"},
                { "tags", new List<object> { "1" } }
            };
            PageMetaData pageMetaData = new PageMetaData(pageData);
            pageMetaData.Id = DefaultPageId;
            items.Add(pageMetaData);

            Dictionary<string, object> data = new();
            SiteMetaData siteMetaData = new SiteMetaData(DefaultSiteId, "", "", "", "", "", data, buildData, items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_PagesWithCorrespondingTagData()
        {
            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));

            TagMetaDataCollection tagMetaDataCollection = new();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = "1";
            tagMetaDataCollection.Add(tagMetaData);
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };

            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                { "baseuri", "http://127.0.0.1" },
                { "uri", "example.html"},
                { "tags", new List<object> { "1" } }
            };
            PageMetaData pageMetaData = new PageMetaData(pageData);
            pageMetaData.Id = DefaultPageId;
            items.Add(pageMetaData);

            SiteMetaData siteMetaData = new SiteMetaData(DefaultSiteId, "", "", "", "", "", data, buildData, items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_ArticlesWithCorrespondingTagData()
        {
            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));

            TagMetaDataCollection tagMetaDataCollection = new();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = "1";
            tagMetaDataCollection.Add(tagMetaData);
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };

            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                { "baseuri", "http://127.0.0.1" },
                { "uri", "example.html"},
                { "tags", new List<object> { "1" } }
            };
            Article pageMetaData = new Article(pageData);
            pageMetaData.Id = DefaultPageId;
            items.Add(pageMetaData);

            SiteMetaData siteMetaData = new SiteMetaData(DefaultSiteId, "", "", "", "", "", data, buildData, items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_ArticlesWithYearData()
        {
            await Task.CompletedTask;
            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));

            static Article Create(string id, DateTimeOffset published)
            {
                Dictionary<string, object?> pageData = new()
                {
                    { "id", id },
                    { "published", published }
                };
                Article pageMetaData = new Article(pageData);
                return pageMetaData;
            }

#pragma warning disable
            List<BasePage> items = [
                Create("a", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)),
                Create("b", new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)),
                Create("c", new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero))
            ];
#pragma warning restore
            Dictionary<string, object> data = new();
            SiteMetaData siteMetaData = new SiteMetaData(DefaultSiteId, "", "", "", "", "", data, buildData, items);
            // await Verifier.Verify(siteMetaData, _VerifySettings);
            SortedDictionary<int, List<PageMetaData>> years = siteMetaData.PagesByYears;
            PageMetaData pageMetaData = years[2025][0];
            PageId id = pageMetaData.Id;
            PageMetaData? retrievedPage = siteMetaData[id];

            PageMetaData? notFound = siteMetaData["123"];

            PageId[] ids = new[] { new PageId("a"), new PageId("b") };
            IEnumerable<PageMetaData> result = siteMetaData[ids];

            foreach (PageMetaData item in result)
            {
            }
        }

        [Fact]
        public async Task Test_ArticlesWithCorrespondingTagData_AsPreview()
        {
            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));

            TagMetaDataCollection tagMetaDataCollection = new();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = "1";
            tagMetaDataCollection.Add(tagMetaData);
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };

            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                { "baseuri", "http://127.0.0.1" },
                { "uri", "example.html"},
                { "tags", new List<object> { "1" } }
            };
            Article pageMetaData = new Article(pageData);
            pageMetaData.Id = DefaultPageId;
            items.Add(pageMetaData);

            SiteMetaData siteMetaData = new SiteMetaData(DefaultSiteId, "", "", "", "", "", data, buildData, items);

            string html = ObjectConversions.ToDiagnosticHtml(siteMetaData, "json");
            await Verifier.Verify(html, _VerifySettings);
        }
    }
}