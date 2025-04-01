// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
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

            Dictionary<AuthorId, Person> authors = ToPersons(renderData.Site);
            Dictionary<OrganizationId, Organization> organizations = ToOrganizations(renderData.Site);

            Type pageType = renderData.Page.GetType();
            LogLdJson(renderData.Page.Uri, renderData.Page.Type);
            Dictionary<Type, Func<BasePage, Thing>> pageParsers = new();
            pageParsers[typeof(PageMetaData)] = Safe((PageMetaData page) => ToWebPage(page, renderData.Site));
            pageParsers[typeof(CollectionPage)] = Safe((CollectionPage page) => ToCollectionPage(page, authors, organizations));
            pageParsers[typeof(ArticleMetaData)] = Safe((ArticleMetaData page) => ToBlogPosting(page, authors, organizations));
            pageParsers[typeof(TalkMetaData)] = Safe((TalkMetaData page) => ToEvent(page, authors));

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

        Thing ToCollectionPage(CollectionPage page, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            if (page.Uri == "blog.html")
            {
                Blog blog = ToBlog(page, authors, organizations);
                return blog;
            }

            Schema.NET.CollectionPage collectionResult = ToCollectionPage(page);
            return collectionResult;
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

        Event ToEvent(BasePage page, Dictionary<AuthorId, Person> authors)
        {
            if (page is not TalkMetaData talk)
            {
                throw new InvalidOperationException();
            }

#pragma warning disable

            PresentationDigitalDocument presentationScheme = new PresentationDigitalDocument();
            presentationScheme.Name = "Slide Deck for Modern Microservices";
            presentationScheme.Url = new Uri("https://cdn.kaylumah.nl/slides/modern-microservices.html");
            presentationScheme.EncodingFormat = "text/html";

            Place placeScheme = new Place();
            // place.Name = "Ilionx Dev Days 2023";

            Event eventScheme = new Event();
            eventScheme.Url = talk.CanonicalUri; // new Uri("https://kaylumah.nl/talks/modern-microservices.html")
            eventScheme.Name = talk.Name; // "Modern Microservices"
            eventScheme.Description =
                talk.Description; // "Talk presented at TechConf 2025 in Amsterdam about migrating .NET monoliths to cloud-native microservices."
            string keywords = string.Join(',', talk.Tags);
            eventScheme.Keywords = keywords;
            eventScheme.WorkPerformed = presentationScheme;
            eventScheme.Location = placeScheme;
            // StartDate = new DateTimeOffset(2025, 5, 21, 14, 30, 0, TimeSpan.Zero),
            // EndDate = new DateTimeOffset(2025, 5, 21, 15, 15, 0, TimeSpan.Zero)

            if (!string.IsNullOrEmpty(talk.Author) && authors.TryGetValue(talk.Author, out Person? personScheme))
            {
                eventScheme.Performer = personScheme;
            }

            /*
             *
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
               }
             */

            return eventScheme;
#pragma warning restore
        }
    }
}
