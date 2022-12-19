// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
using Schema.NET;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo;

public static class PageMetaDataExtensions
{
    public static BlogPosting ToBlogPosting(this PageMetaData page, Dictionary<string, Person> persons, Dictionary<string, Organization> organizations)
    {
        var blogPost = new BlogPosting
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

    public static IEnumerable<BlogPosting> ToBlogPostings(this IEnumerable<PageMetaData> pages, Dictionary<string, Person> persons, Dictionary<string, Organization> organizations)
    {
        return pages.Select(page => page.ToBlogPosting(persons, organizations));
    }
}
