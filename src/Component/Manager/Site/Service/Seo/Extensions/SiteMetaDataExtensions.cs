// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;
using CollectionPage = Ssg.Extensions.Metadata.Abstractions.CollectionPage;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    public static class SiteMetaDataExtensions
    {
        public static Dictionary<AuthorId, Person> ToPersons(this SiteMetaData source)
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

        public static Dictionary<OrganizationId, Organization> ToOrganizations(this SiteMetaData source)
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

        public static Schema.NET.CollectionPage ToCollectionPage(this CollectionPage page)
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

        public static Blog ToBlog(this CollectionPage page, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
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

        public static BlogPosting ToBlogPosting(this ArticleMetaData page, Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
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
