// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.Manager.Site.Service;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Scriban.Runtime;
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
            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_OnlyTags()
        {
            TagMetaDataCollection tagMetaDataCollection = new();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = "1";
            tagMetaDataCollection.Add(tagMetaData);
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };

            List<BasePage> items = new();
            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_OnlyPages()
        {
            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                { "id", DefaultPageId },
                { "baseuri", "http://127.0.0.1" },
                { "uri", "example.html"},
                { "tags", new List<object> { "1" } }
            };
            PageMetaData pageMetaData = new PageMetaData(pageData);
            items.Add(pageMetaData);

            Dictionary<string, object> data = new();
            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, items: items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_PagesWithCorrespondingTagData()
        {
            TagMetaDataCollection tagMetaDataCollection = new();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = "1";
            tagMetaDataCollection.Add(tagMetaData);
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };

            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                { "id", DefaultPageId },
                { "baseuri", "http://127.0.0.1" },
                { "uri", "example.html"},
                { "tags", new List<object> { "1" } }
            };
            PageMetaData pageMetaData = new PageMetaData(pageData);
            items.Add(pageMetaData);

            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data, items: items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_ArticlesWithCorrespondingTagData()
        {
            TagMetaDataCollection tagMetaDataCollection = new();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = "1";
            tagMetaDataCollection.Add(tagMetaData);
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };

            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                { "id", DefaultPageId },
                { "baseuri", "http://127.0.0.1" },
                { "uri", "example.html"},
                { "tags", new List<object> { "1" } }
            };
            ArticlePublicationPageMetaData pagePublicationPageMetaData = new ArticlePublicationPageMetaData(pageData);
            items.Add(pagePublicationPageMetaData);

            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data, items: items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_ArticlesWithCorrespondingTagData_AsPreview()
        {
            TagMetaDataCollection tagMetaDataCollection = new();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = "1";
            tagMetaDataCollection.Add(tagMetaData);
            Dictionary<string, object> data = new() { { "tags", tagMetaDataCollection } };

            List<BasePage> items = new();
            Dictionary<string, object?> pageData = new()
            {
                { "id", DefaultPageId },
                { "organization", "001" },
                { "author", "N/A "},
                { "baseuri", "http://127.0.0.1" },
                { "uri", "example.html"},
                { "tags", new List<object> { "1" } }
            };
            ArticlePublicationPageMetaData pagePublicationPageMetaData = new ArticlePublicationPageMetaData(pageData);
            items.Add(pagePublicationPageMetaData);

            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data, items: items);

            string html = ObjectConversions.ToDiagnosticHtml(siteMetaData, "json");
            await Verifier.Verify(html, _VerifySettings);
        }

        [Fact]
        public async Task Test_ArticlesWithCorrespondingYears()
        {
            Dictionary<string, object> data = new();
            List<BasePage> items = new();

            static ArticlePublicationPageMetaData CreateArticle(string pageId, DateTimeOffset published)
            {
                Dictionary<string, object?> pageData = new()
                {
                    { "id", pageId },
                    { "baseuri", "http://127.0.0.1" },
                    { "uri", "example.html"},
                    { "published", published }
                };
                ArticlePublicationPageMetaData pagePublicationPageMetaData = new ArticlePublicationPageMetaData(pageData);
                return pagePublicationPageMetaData;
            }

#pragma warning disable
            items.Add(CreateArticle("4", new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)));
            items.Add(CreateArticle("3", new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero)));
            items.Add(CreateArticle("2", new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero)));
            items.Add(CreateArticle("1", new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)));
#pragma warning restore

            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data, items: items);
            await Verifier.Verify(siteMetaData, _VerifySettings);
        }

        [Fact]
        public async Task Test_Scriban_Handles_CrossSections()
        {
            Dictionary<string, object> data = new();
            Dictionary<string, object?> pageData = new()
            {
                { "id", "1" },
                { "published", new DateTimeOffset(2025,1,1, 0, 0,0, TimeSpan.Zero) }
            };
            ArticlePublicationPageMetaData pagePublicationPageMetaData = new ArticlePublicationPageMetaData(pageData);
            List<BasePage> items = new List<BasePage>();
            items.Add(pagePublicationPageMetaData);
            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data, items: items);

            RenderData renderData = new RenderData(siteMetaData, null!);
            string content =
                """
                {% assign perYear = site.pagesbyyears[2025] %}
                {% assign pages = site[perYear] %}
                {% for tag in pages %}
                {{ tag.published }}
                {% endfor %}
                """;

            string result = await Render(content, renderData);
        }

        [Fact]
        public async Task Test_Scriban_Handles_Diagnostic_Minimal()
        {
            Dictionary<string, object> data = new();
            Dictionary<string, object?> pageData = new()
            {
                { "organization", "001" },
                { "author", "N/A"},
                { "baseuri", "http://127.0.0.1" },
                { "uri", "1.html "},
                { "id", "1" },
                { "published", new DateTimeOffset(2025,1,1, 0, 0,0, TimeSpan.Zero) }
            };
            ArticlePublicationPageMetaData pagePublicationPageMetaData = new ArticlePublicationPageMetaData(pageData);
            List<BasePage> items = new List<BasePage>();
            items.Add(pagePublicationPageMetaData);
            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data, items: items);
            RenderData renderData = new RenderData(siteMetaData, pagePublicationPageMetaData);
            string content =
                """
                {{ site | to_diagnostic_html "piet" }}
                """;

            string result = await Render(content, renderData);
        }

        [Fact]
        public async Task Test_Scriban_Handles_AuthorDictionary()
        {
            AuthorMetaDataCollection authorMetaDataCollection = new AuthorMetaDataCollection();
            AuthorMetaData authorMetaData = new AuthorMetaData();
            authorMetaData.Id = new AuthorId("§");
            authorMetaDataCollection.Add(authorMetaData);

            Dictionary<string, object> data = new();
            data["authors"] = authorMetaDataCollection;
            Dictionary<string, object?> pageData = new()
            {
                { "organization", "001" },
                { "author", "002" },
                { "baseuri", "http://127.0.0.1" },
                { "uri", "1.html "},
                { "id", "1" },
                { "published", new DateTimeOffset(2025,1,1, 0, 0,0, TimeSpan.Zero) }
            };
            ArticlePublicationPageMetaData pagePublicationPageMetaData = new ArticlePublicationPageMetaData(pageData);
            List<BasePage> items = new List<BasePage>();
            items.Add(pagePublicationPageMetaData);
            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data, items: items);
            RenderData renderData = new RenderData(siteMetaData, pagePublicationPageMetaData);
            string content =
                """
                {{ site | to_diagnostic_html "piet" }}
                """;

            string result = await Render(content, renderData);
        }

        [Fact]
        public async Task Test_Scriban_Handles_OrganizationDictionary()
        {
            OrganizationMetaDataCollection organizationMetaDataCollection = new OrganizationMetaDataCollection();
            OrganizationMetaData organizationMetadata = new OrganizationMetaData();
            organizationMetadata.Id = new OrganizationId("§");
            organizationMetaDataCollection.Add(organizationMetadata);

            BuildData buildData = (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));
            Dictionary<string, object> data = new();
            data["organizations"] = organizationMetaDataCollection;
            Dictionary<string, object?> pageData = new()
            {
                { "organization", "001" },
                { "author", "002" },
                { "baseuri", "http://127.0.0.1" },
                { "uri", "1.html "},
                { "id", "1" },
                { "published", new DateTimeOffset(2025,1,1, 0, 0,0, TimeSpan.Zero) }
            };
            ArticlePublicationPageMetaData pagePublicationPageMetaData = new ArticlePublicationPageMetaData(pageData);
            List<BasePage> items = new List<BasePage>();
            items.Add(pagePublicationPageMetaData);
            SiteMetaData siteMetaData = CreateSiteMetaData(DefaultSiteId, data: data, items: items);
            RenderData renderData = new RenderData(siteMetaData, pagePublicationPageMetaData);
            string content =
                """
                {{ site | to_diagnostic_html "piet" }}
                """;

            string result = await Render(content, renderData);
        }

        [Fact]
        public async Task Test_Scriban_Handles_TagDictionary()
        {
            TagMetaDataCollection tagMetaDataCollection = new TagMetaDataCollection();
            TagMetaData tagMetaData = new TagMetaData();
            tagMetaData.Id = new TagId("§");
            tagMetaDataCollection.Add(tagMetaData);

            Dictionary<string, object> data = new();
            data["tags"] = tagMetaDataCollection;
            Dictionary<string, object?> pageData = new()
            {
                { "organization", "001" },
                { "author", "002" },
                { "baseuri", "http://127.0.0.1" },
                { "uri", "1.html "},
                { "id", "1" },
                { "published", new DateTimeOffset(2025,1,1, 0, 0,0, TimeSpan.Zero) }
            };
            ArticlePublicationPageMetaData pagePublicationPageMetaData = new ArticlePublicationPageMetaData(pageData);
            List<BasePage> items = new List<BasePage>();
            items.Add(pagePublicationPageMetaData);
            SiteMetaData siteMetaData =
                CreateSiteMetaData(siteId: DefaultSiteId, data: data, items: items);
            RenderData renderData = new RenderData(siteMetaData, pagePublicationPageMetaData);
            string content =
                """
                {{ site | to_diagnostic_html "piet" }}
                """;

            string result = await Render(content, renderData);
        }

        static async Task<string> Render(string content, RenderData renderData)
        {
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
            scriptObject.Import(typeof(ObjectConversions));
            context.PushGlobal(scriptObject);

            string renderedContent = await liquidTemplate.RenderAsync(context);
            return renderedContent;
        }

        static SiteMetaData CreateSiteMetaData(
            SiteId? siteId,
            string? title = null,
            string? description = null,
            string? language = null,
            string? author = null,
            string? url = null,
            Dictionary<string, object>? data = null,
            BuildData? buildData = null,
            List<BasePage>? items = null)
        {
            siteId ??= new SiteId("<UNK>");
            title ??= string.Empty;
            description ??= string.Empty;
            language ??= string.Empty;
            author ??= string.Empty;
            url ??= string.Empty;
            buildData ??= (BuildData)RuntimeHelpers.GetUninitializedObject(typeof(BuildData));
            data ??= new();
            items ??= new();

            SiteMetaData siteMetaData = new SiteMetaData(
                siteId.Value,
                title: title,
                description: description,
                language: language,
                author: author,
                url: url,
                data: data,
                buildData: buildData,
                items: items);
            return siteMetaData;
        }
    }
}