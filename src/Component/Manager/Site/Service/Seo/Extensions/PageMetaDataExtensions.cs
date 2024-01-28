// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Utilities;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    public static class PageMetaDataExtensions
    {
        public static BlogPosting ToBlogPosting(this PageMetaData page, Dictionary<AuthorId, Person> persons, Dictionary<string, Organization> organizations)
        {
            BlogPosting blogPost = new BlogPosting
            {
                // Id = new Uri(GlobalFunctions.AbsoluteUrl(renderData.page.Uri)),
                MainEntityOfPage = new Values<ICreativeWork, Uri>(new Uri(GlobalFunctions.AbsoluteUrl(page.Uri))),
                Headline = page.Title,
#pragma warning disable RS0030 // datetime is expected here
                DatePublished = page.Published.DateTime,
                DateModified = page.Modified.DateTime
#pragma warning restore RS0030 // datetime is expected here
            };

            if (!string.IsNullOrEmpty(page.Image))
            {
                blogPost.Image = new Values<IImageObject, Uri>(new Uri(GlobalFunctions.AbsoluteUrl((string)page.Image)));
            }

            if (!string.IsNullOrEmpty(page.Author) && persons.TryGetValue(page.Author, out Person person))
            {
                blogPost.Author = person;
            }

            if (!string.IsNullOrEmpty(page.Organization) && organizations.TryGetValue(page.Organization, out Organization organization))
            {
                blogPost.Publisher = organization;
            }

            return blogPost;
        }

        public static IEnumerable<BlogPosting> ToBlogPostings(this IEnumerable<PageMetaData> pages, Dictionary<AuthorId, Person> persons, Dictionary<string, Organization> organizations)
        {
            return pages.Select(page => page.ToBlogPosting(persons, organizations));
        }
    }
}
