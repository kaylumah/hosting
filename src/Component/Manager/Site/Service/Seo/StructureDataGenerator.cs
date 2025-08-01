﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Extensions.Metadata.Abstractions;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Microsoft.Extensions.Logging;
using Schema.NET;

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
            System.Text.Json.JsonSerializerOptions settings = new System.Text.Json.JsonSerializerOptions();
            settings.AllowTrailingCommas = true;
            settings.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
            settings.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            settings.WriteIndented = true;

            Dictionary<AuthorId, Person> authors = ToPersons(renderData.Site);
            Dictionary<OrganizationId, Organization> organizations = ToOrganizations(renderData.Site);

            Type pageType = renderData.Page.GetType();
            LogLdJson(renderData.Page.Uri, renderData.Page.Type);
            Dictionary<Type, Func<BasePage, Thing>> pageParsers = new();
            pageParsers[typeof(PageMetaData)] = Safe((PageMetaData page) => ToWebPage(page, renderData.Site));
            pageParsers[typeof(CollectionPageMetaData)] = Safe((CollectionPageMetaData page) => ToCollectionPage(page, authors, organizations));
            pageParsers[typeof(ArticlePublicationPageMetaData)] = Safe((ArticlePublicationPageMetaData page) => ToBlogPosting(page, authors, organizations));
            pageParsers[typeof(TalkPublicationPageMetaData)] = Safe((TalkPublicationPageMetaData page) => ToEvent(page, authors));

            bool hasConverter = pageParsers.TryGetValue(pageType, out Func<BasePage, Thing>? parser);
            if (hasConverter && parser != null)
            {
                Thing scheme = parser(renderData.Page);
                string json = scheme.ToString(settings);
                return json;
            }

            return string.Empty;
        }

        static Func<BasePage, Thing> Safe<TPage>(Func<TPage, Thing> handler)
            where TPage : BasePage
        {
            return page =>
            {
                if (page is not TPage typed)
                {
                    throw new InvalidCastException(
                        $"Expected page of type {typeof(TPage).Name}, but got {page.GetType().Name}");
                }

                Thing result = handler(typed);
                return result;
            };
        }

        Dictionary<AuthorId, Person> ToPersons(SiteMetaData source)
        {
            Dictionary<AuthorId, Person> result;

            if (source.AuthorMetaData == null)
            {
                result = [];
            }
            else
            {
                result = source.AuthorMetaData
                    .ToDictionary(x => x.Id, x =>
                    {
                        List<Uri> uris = new List<Uri>();

                        if (!string.IsNullOrEmpty(x.Links.Linkedin))
                        {
                            Uri linkedinUri = new Uri(x.Links.LinkedinProfileUrl!);
                            uris.Add(linkedinUri);
                        }

                        if (!string.IsNullOrEmpty(x.Links.Twitter))
                        {
                            Uri twitterUri = new Uri(x.Links.TwitterProfileUrl!);
                            uris.Add(twitterUri);
                        }

                        Person person = new Person();
                        person.Name = x.FullName;
                        person.Email = x.Email;
                        person.SameAs = new OneOrMany<Uri>(uris);

                        if (!string.IsNullOrEmpty(x.Uri))
                        {
                            Uri personUri = source.AbsoluteUri(x.Uri);
                            person.Url = personUri;
                        }

                        if (!string.IsNullOrEmpty(x.Picture))
                        {
                            Uri image = source.AbsoluteUri(x.Picture);
                            person.Image = new Values<IImageObject, Uri>(image);
                        }

                        return person;
                    });
            }

            return result;
        }

        Dictionary<OrganizationId, Organization> ToOrganizations(SiteMetaData source)
        {
            Dictionary<OrganizationId, Organization> result;
            if (source.OrganizationMetaData == null)
            {
                result = [];
            }
            else
            {
                result = source.OrganizationMetaData
                    .ToDictionary(x => x.Id, x =>
                    {

                        List<Uri> uris = new List<Uri>
                        {
                            new Uri($"https://www.linkedin.com/company/{x.Linkedin}")
                        };

                        Organization organization = new Organization();
                        organization.Name = x.FullName;
#pragma warning disable RS0030 // not time based
                        organization.FoundingDate = x.Founded.Date;
#pragma warning restore RS0030
                        organization.SameAs = new OneOrMany<Uri>(uris);

                        if (!string.IsNullOrEmpty(x.Logo))
                        {
                            Uri logoUri = source.AbsoluteUri(x.Logo);
                            organization.Logo =
                                new Values<IImageObject, Uri>(logoUri);
                        }

                        return organization;
                    });
            }

            return result;
        }

        WebPage ToWebPage(PageMetaData pageMetaData, SiteMetaData site)
        {
            WebSite scheme = new WebSite();
            scheme.Name = site.Title;
            scheme.Url = new Uri(site.Url);
            WebPage webPageScheme = new WebPage();
            webPageScheme.Name = pageMetaData.Title;
            webPageScheme.Url = pageMetaData.CanonicalUri;
            webPageScheme.Description = pageMetaData.Description;
            webPageScheme.IsPartOf = scheme;
            return webPageScheme;
        }

        Thing ToCollectionPage(CollectionPageMetaData pageMetaData, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            if (pageMetaData.Uri == "blog.html")
            {
                Blog blog = ToBlog(pageMetaData, authors, organizations);
                return blog;
            }

            CollectionPage collectionResult = ToCollectionPage(pageMetaData);
            return collectionResult;
        }

        CollectionPage ToCollectionPage(CollectionPageMetaData pageMetaData)
        {
            List<ICreativeWork> creativeWorks = new List<ICreativeWork>();
            IEnumerable<ArticlePublicationPageMetaData> articles = pageMetaData.RecentArticles;
            foreach (ArticlePublicationPageMetaData article in articles)
            {
                BlogPosting blogPosting = new BlogPosting();
                blogPosting.Headline = article.Title;
                blogPosting.Url = article.CanonicalUri;
                creativeWorks.Add(blogPosting);
            }

            CollectionPage collectionPage = new CollectionPage();
            collectionPage.Url = pageMetaData.CanonicalUri;
            collectionPage.Name = pageMetaData.Title;
            collectionPage.Description = pageMetaData.Description;
            string keywords = string.Join(',', pageMetaData.Keywords);
            collectionPage.Keywords = keywords;
            collectionPage.HasPart = new OneOrMany<ICreativeWork>(creativeWorks);
            return collectionPage;
        }

        Blog ToBlog(CollectionPageMetaData pageMetaData, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            List<BlogPosting> posts = new List<BlogPosting>();
            IEnumerable<ArticlePublicationPageMetaData> articles = pageMetaData.RecentArticles;
            foreach (ArticlePublicationPageMetaData article in articles)
            {
                BlogPosting blogPosting = ToBlogPosting(article, authors, organizations);
                posts.Add(blogPosting);
            }

            Blog blog = new Blog();
            blog.Url = pageMetaData.CanonicalUri;
            blog.Name = pageMetaData.Title;
            blog.Description = pageMetaData.Description;
            string keywords = string.Join(',', pageMetaData.Keywords);
            blog.Keywords = keywords;
#pragma warning disable RS0030 // DatePublished can be datetime so it is a false positive
            blog.DatePublished = pageMetaData.Published;
            blog.DateModified = pageMetaData.Modified;
#pragma warning restore RS0030
            blog.BlogPost = new OneOrMany<IBlogPosting>(posts);
            return blog;
        }

        BlogPosting ToBlogPosting(ArticlePublicationPageMetaData page, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            Uri pageUri = page.CanonicalUri;
            BlogPosting blogPost = new BlogPosting();

            blogPost.MainEntityOfPage = pageUri;
            blogPost.Url = pageUri;

#pragma warning disable RS0030 // DatePublished can be datetime so it is a false positive
            blogPost.DatePublished = page.Published;
            blogPost.DateModified = page.Modified;
#pragma warning restore RS0030

            blogPost.Headline = page.Title;
            // blogPost.Name = page.Title;
            blogPost.Description = page.Description;
            string keywords = string.Join(',', page.Tags);
            blogPost.Keywords = keywords;

            if (page.WebImage != null)
            {
                blogPost.Image = new Values<IImageObject, Uri>(page.WebImage);
            }

            if (!string.IsNullOrEmpty(page.Author) && authors.TryGetValue(page.Author, out Person? person))
            {
                blogPost.Author = person;
            }

            if (!string.IsNullOrEmpty(page.Organization) && organizations.TryGetValue(page.Organization, out Organization? organization))
            {
                blogPost.Publisher = organization;
            }

            blogPost.InLanguage = page.Language;
            blogPost.WordCount = page.NumberOfWords;
            blogPost.TimeRequired = page.Duration;
            return blogPost;
        }

        Event ToEvent(BasePage page, Dictionary<AuthorId, Person> authors)
        {
            if (page is not TalkPublicationPageMetaData talk)
            {
                throw new InvalidOperationException();
            }

            PresentationDigitalDocument presentationScheme = new PresentationDigitalDocument();
            presentationScheme.Name = talk.Title; //"Slide Deck for Modern Microservices";
            presentationScheme.Url = talk.PresentationUri;
            presentationScheme.EncodingFormat = "text/html";

            Place placeScheme = new Place();
            placeScheme.Name = talk.Location;

            Event eventScheme = new Event();
            eventScheme.Url = talk.CanonicalUri;
            eventScheme.Name = talk.Name;
            eventScheme.Description = talk.Description;
            string keywords = string.Join(',', talk.Tags);
            eventScheme.Keywords = keywords;
            eventScheme.WorkPerformed = presentationScheme;
            eventScheme.Location = placeScheme;

#pragma warning disable RS0030 // DatePublished can be datetime so it is a false positive
            eventScheme.StartDate = talk.EventDate;
#pragma warning restore RS0030

            if (!string.IsNullOrEmpty(talk.Author) && authors.TryGetValue(talk.Author, out Person? personScheme))
            {
                eventScheme.Performer = personScheme;
            }

            return eventScheme;
        }
    }
}
