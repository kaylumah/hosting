﻿// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Kaylumah.Ssg.Utilities;
using Microsoft.Extensions.Logging;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;
using Article = Ssg.Extensions.Metadata.Abstractions.Article;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    public partial class StructureDataGenerator
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Trace,
            Message = "Attempting LdJson `{Path}` and `{Type}`")]
        private partial void LogLdJson(string path, string type);

        readonly ILogger _Logger;

        public StructureDataGenerator(ILogger<StructureDataGenerator> logger)
        {
            _Logger = logger;
        }

        public string ToLdJson(RenderData renderData)
        {
            // Check https://search.google.com/test/rich-results to validate LDJson
            ArgumentNullException.ThrowIfNull(renderData);
            JsonSerializerOptions settings = new JsonSerializerOptions();
            settings.AllowTrailingCommas = true;
            settings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
            settings.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            settings.WriteIndented = true;

            Dictionary<AuthorId, Person> authors = renderData.Site.ToPersons();
            Dictionary<OrganizationId, Organization> organizations = renderData.Site.ToOrganizations();

            if (renderData.Page is PageMetaData pageMetaData)
            {
                LogLdJson(pageMetaData.Uri, pageMetaData.Type);
                if (pageMetaData.IsArticle())
                {
                    BlogPosting blogPost = ToBlogPosting(pageMetaData, authors, organizations);
                    string ldjson = blogPost.ToString(settings);
                    return ldjson;
                }
                else if (pageMetaData.IsCollection() && "blog.html".Equals(pageMetaData.Uri, StringComparison.Ordinal))
                {
                    List<Article> articles = renderData.Site.FeaturedArticles.ToList();

                    Blog blog = ToBlog(pageMetaData, articles, authors, organizations);
                    string ldjson = blog.ToString(settings);
                    return ldjson;
                }
            }

            string result = string.Empty;
            return result;
        }

        Blog ToBlog(PageMetaData page, List<Article> articles, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            Uri pageUri = GlobalFunctions.AbsoluteUri(page.Uri);
            List<BlogPosting> posts = new List<BlogPosting>();
            foreach (PageMetaData article in articles)
            {
                BlogPosting blogPosting = ToBlogPosting(article, authors, organizations);
                posts.Add(blogPosting);
            }

            Blog blog = new Blog();
            blog.Url = pageUri;
#pragma warning disable RS0030 // DatePublished can be datetime so it is a false positive
            blog.DatePublished = page.Published;
            blog.DateModified = page.Modified;
#pragma warning restore RS0030
            blog.BlogPost = new OneOrMany<IBlogPosting>(posts);
            return blog;
        }

        BlogPosting ToBlogPosting(PageMetaData page, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            Uri pageUri = GlobalFunctions.AbsoluteUri(page.Uri);
            BlogPosting blogPost = new BlogPosting();

            blogPost.MainEntityOfPage = pageUri;
            blogPost.Url = pageUri;

#pragma warning disable RS0030 // DatePublished can be datetime so it is a false positive
            blogPost.DatePublished = page.Published;
            blogPost.DateModified = page.Modified;
#pragma warning restore RS0030

            blogPost.Headline = page.Title;
            blogPost.Description = page.Description;
            string keywords = string.Join(',', page.Tags);
            blogPost.Keywords = keywords;

            if (!string.IsNullOrEmpty(page.Image))
            {
                Uri imageUri = GlobalFunctions.AbsoluteUri(page.Image);
                blogPost.Image = new Values<IImageObject, Uri>(imageUri);
            }

            if (!string.IsNullOrEmpty(page.Author) && authors.TryGetValue(page.Author, out Person? person))
            {
                blogPost.Author = person;
            }

            if (!string.IsNullOrEmpty(page.Organization) && organizations.TryGetValue(page.Organization, out Organization? organization))
            {
                blogPost.Publisher = organization;
            }

            return blogPost;
        }
    }
}
