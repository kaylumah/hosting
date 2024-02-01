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
        public static BlogPosting ToBlogPosting(this PageMetaData page, Dictionary<AuthorId, Person> persons, Dictionary<OrganizationId, Organization> organizations)
        {
            BlogPosting blogPost = new BlogPosting();
            // blogPost.Id = new Uri(GlobalFunctions.AbsoluteUrl(renderData.page.Uri)),
            Uri pageUri = GlobalFunctions.AbsoluteUri(page.Uri);
            blogPost.MainEntityOfPage = new Values<ICreativeWork, Uri>(pageUri);
            blogPost.Headline = page.Title;
            #pragma warning disable RS0030 // datetime is expected here
            blogPost.DatePublished = page.Published.DateTime;
            blogPost.DateModified = page.Modified.DateTime;

            if (!string.IsNullOrEmpty(page.Image))
            {
                Uri imageUri = GlobalFunctions.AbsoluteUri(page.Image);
                blogPost.Image = new Values<IImageObject, Uri>(imageUri);
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

        public static IEnumerable<BlogPosting> ToBlogPostings(this IEnumerable<PageMetaData> pages, Dictionary<AuthorId, Person> persons, Dictionary<OrganizationId, Organization> organizations)
        {
            IEnumerable<BlogPosting> result = pages.Select(page => page.ToBlogPosting(persons, organizations));
            return result;
        }
    }
}
