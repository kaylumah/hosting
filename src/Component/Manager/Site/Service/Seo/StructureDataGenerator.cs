// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Microsoft.Extensions.Logging;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    public partial class StructureDataGenerator
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Trace,
            Message = "Attempting LdJson `{Path}` and `{Type:g}`")]
        private partial void LogLdJson(string path, ContentType type);

        readonly ILogger _Logger;

        public StructureDataGenerator(ILogger<StructureDataGenerator> logger)
        {
            _Logger = logger;
        }

        public string ToLdJson(RenderData renderData)
        {
            ArgumentNullException.ThrowIfNull(renderData);
            JsonSerializerOptions settings = new JsonSerializerOptions();
            settings.AllowTrailingCommas = true;
            settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            settings.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            settings.WriteIndented = true;

            System.Collections.Generic.Dictionary<AuthorId, Person> authors = renderData.Site.ToPersons();
            System.Collections.Generic.Dictionary<OrganizationId, Organization> organizations = renderData.Site.ToOrganizations();
            LogLdJson(renderData.Page.Uri, renderData.Page.Type);
            if (renderData.Page.Type == ContentType.Article)
            {
                BlogPosting blogPost = renderData.Page.ToBlogPosting(authors, organizations);
                string ldjson = blogPost.ToString(settings);
                return ldjson;
            }
            else if (renderData.Page.Type == ContentType.Page && "blog.html".Equals(renderData.Page.Uri, StringComparison.Ordinal))
            {
                System.Collections.Generic.List<BlogPosting> posts = renderData.Site.Pages
                    .IsArticle()
                    .IsFeatured()
                    .ByRecentlyPublished()
                    .ToBlogPostings(authors, organizations)
                    .ToList();
                Blog blog = new Blog();
                blog.BlogPost = new OneOrMany<IBlogPosting>(posts);
                string ldjson = blog.ToString(settings);
                return ldjson;
            }

            string result = string.Empty;
            return result;
        }
    }
}
