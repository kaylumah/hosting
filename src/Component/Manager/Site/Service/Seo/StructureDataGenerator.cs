// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Kaylumah.Ssg.Manager.Site.Service.RenderEngine;
using Microsoft.Extensions.Logging;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;
using CollectionPage = Ssg.Extensions.Metadata.Abstractions.CollectionPage;

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

            Dictionary<AuthorId, Person> authors = renderData.Site.ToPersons();
            Dictionary<OrganizationId, Organization> organizations = renderData.Site.ToOrganizations();

            if (renderData.Page is ArticleMetaData article)
            {
                LogLdJson(article.Uri, article.Type);
                BlogPosting blogPostScheme = ToBlogPosting(article, authors, organizations);
                string blogPostSchemeJson = blogPostScheme.ToString(settings);
                return blogPostSchemeJson;
            }
            else if (renderData.Page is TalkMetaData talk)
            {
                /*
                PresentationDigitalDocument document = new PresentationDigitalDocument();
                document.Name = "Modern Microservices Slide Deck";
                document.Url = new Uri("https://cdn.kaylumah.nl/slides/modern-microservices.html");
                document.EncodingFormat = "text/html";

                Place place = new Place();
                // place.Name = "Ilionx Dev Days 2023";
                // place.Address

                Event eventScheme = new Event();
                eventScheme.Url = talk.CanonicalUri;
                eventScheme.Name = talk.Title;
                eventScheme.Description = talk.Description;
#pragma warning disable RS0030 // DatePublished can be datetime so it is a false positive
                eventScheme.StartDate = new DateTimeOffset(2025, 5, 21, 14, 30, 0, TimeSpan.Zero);
#pragma warning restore RS0030

                if (!string.IsNullOrEmpty(talk.Author) && authors.TryGetValue(talk.Author, out Person? person))
                {
                    eventScheme.Performer = person;
                }

                eventScheme.Location = place;

                eventScheme.WorkPerformed = new OneOrMany<ICreativeWork>(document);
                string eventSchemeJson = eventScheme.ToString(settings);
                return eventSchemeJson;
                */
#pragma warning disable
                var talkData = new Event
                {
                    Name = "Modern Microservices",
                    StartDate = new DateTimeOffset(2025, 5, 21, 14, 30, 0, TimeSpan.Zero),
                    EndDate = new DateTimeOffset(2025, 5, 21, 15, 15, 0, TimeSpan.Zero),
                    Url = new Uri("https://kaylumah.nl/talks/modern-microservices.html"),
                    Description = "Talk presented at TechConf 2025 in Amsterdam about migrating .NET monoliths to cloud-native microservices.",
                    Performer = new Person
                    {
                        Name = "Your Name",
                        Url = new Uri("https://kaylumah.nl/about")
                    },
                    Location = new Place
                    {
                        Name = "Amsterdam RAI Conference Centre",
                        Address = new PostalAddress
                        {
                            AddressLocality = "Amsterdam",
                            AddressCountry = "NL"
                        },
                        Geo = new GeoCoordinates
                        {
                            Latitude = 52.3411,
                            Longitude = 4.8884
                        }
                    },
                    // EventAttendanceMode = EventAttendanceMode.MixedEventAttendanceMode,
                    EventStatus = EventStatusType.EventScheduled,
                    WorkPerformed = new PresentationDigitalDocument
                    {
                        Name = "Slide Deck for Modern Microservices",
                        Url = new Uri("https://cdn.kaylumah.nl/slides/modern-microservices.html"),
                        EncodingFormat = "text/html",
                        Encoding = new MediaObject
                        {
                            ContentUrl = new Uri("https://cdn.kaylumah.nl/slides/modern-microservices.pdf"),
                            EncodingFormat = "application/pdf"
                        }
                    }
                };
                return talkData.ToString(settings);
#pragma warning restore
            }
            else if (renderData.Page is CollectionPage collectionPage)
            {
                if ("blog.html".Equals(collectionPage.Uri, StringComparison.Ordinal))
                {
                    Blog blogScheme = ToBlog(collectionPage, authors, organizations);
                    string blogSchemeJson = blogScheme.ToString(settings);
                    return blogSchemeJson;
                }

                Schema.NET.CollectionPage collectionScheme = ToCollectionPage(collectionPage);
                string collectionSchemeJson = collectionScheme.ToString(settings);
                return collectionSchemeJson;
            }
            else if (renderData.Page is PageMetaData page)
            {
                WebSite scheme = new WebSite();
                scheme.Name = renderData.Site.Title;
                scheme.Url = new Uri(renderData.Site.Url);
                WebPage webPageScheme = new WebPage();
                webPageScheme.Name = page.Title;
                webPageScheme.Url = page.CanonicalUri;
                webPageScheme.Description = page.Description;
                webPageScheme.IsPartOf = scheme;
                string webPageSchemeJson = webPageScheme.ToString(settings);
                return webPageSchemeJson;
            }

            string result = string.Empty;
            return result;
        }

        Schema.NET.CollectionPage ToCollectionPage(CollectionPage page)
        {
            List<ICreativeWork> creativeWorks = new List<ICreativeWork>();
            IEnumerable<ArticleMetaData> articles = page.RecentArticles;
            foreach (ArticleMetaData article in articles)
            {
                BlogPosting blogPosting = new BlogPosting();
                blogPosting.Headline = article.Title;
                blogPosting.Url = article.CanonicalUri;
                creativeWorks.Add(blogPosting);
            }

            Schema.NET.CollectionPage collectionPage = new Schema.NET.CollectionPage();
            collectionPage.Url = page.CanonicalUri;
            collectionPage.Name = page.Title;
            collectionPage.Description = page.Description;
            string keywords = string.Join(',', page.Tags);
            collectionPage.Keywords = keywords;
            collectionPage.HasPart = new OneOrMany<ICreativeWork>(creativeWorks);
            return collectionPage;
        }

        Blog ToBlog(CollectionPage page, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            List<BlogPosting> posts = new List<BlogPosting>();
            IEnumerable<ArticleMetaData> articles = page.RecentArticles;
            foreach (ArticleMetaData article in articles)
            {
                BlogPosting blogPosting = ToBlogPosting(article, authors, organizations);
                posts.Add(blogPosting);
            }

            Blog blog = new Blog();
            blog.Url = page.CanonicalUri;
            blog.Name = page.Title;
            blog.Description = page.Description;
            string keywords = string.Join(',', page.Tags);
            blog.Keywords = keywords;
#pragma warning disable RS0030 // DatePublished can be datetime so it is a false positive
            blog.DatePublished = page.Published;
            blog.DateModified = page.Modified;
#pragma warning restore RS0030
            blog.BlogPost = new OneOrMany<IBlogPosting>(posts);
            return blog;
        }

        BlogPosting ToBlogPosting(ArticleMetaData page, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
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

            // blogPost.Name = page.Title;
            blogPost.InLanguage = page.Language;
            blogPost.WordCount = page.NumberOfWords;
            blogPost.TimeRequired = page.Duration;
            return blogPost;
        }
    }
}
