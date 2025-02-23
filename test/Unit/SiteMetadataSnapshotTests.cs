// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kaylumah.Ssg.Manager.Site.Service;
using Scriban.Runtime;
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

        [Fact(Skip = "just for demo")]
        public async Task Test_Scriban_Handles_NewIds()
        {
            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));
            Dictionary<string, object> data = new();
            Dictionary<string, object?> pageData = new()
            {
                { "id", "1" },
                { "published", new DateTimeOffset(2025,1,1, 0, 0,0, TimeSpan.Zero) }
            };
            Article pageMetaData = new Article(pageData);
            List<BasePage> items = new List<BasePage>();
            items.Add(pageMetaData);
            SiteMetaData siteMetaData = new SiteMetaData(DefaultSiteId, "", "", "", "", "", data, buildData, items);

            object renderData = new { site = siteMetaData };

            string content =
                """
                {% assign perYear = site.pagesbyyears[2025] %}
                {% assign pages = site[perYear] %}
                {% for tag in pages %}
                {{ tag.published }}
                {% endfor %}
                """;
            Scriban.Template liquidTemplate = Scriban.Template.ParseLiquid(content);
            Scriban.LiquidTemplateContext context = new Scriban.LiquidTemplateContext();
            context.MemberRenamer = member =>
            {
                // alternative for the lowercase dictionary
                string result = member.Name.ToLower(CultureInfo.InvariantCulture);
                return result;
            };

            ScriptObject scriptObject = new ScriptObject();
            scriptObject.Import(renderData);
            context.PushGlobal(scriptObject);

            string renderedContent = await liquidTemplate.RenderAsync(context);
        }
    }
}