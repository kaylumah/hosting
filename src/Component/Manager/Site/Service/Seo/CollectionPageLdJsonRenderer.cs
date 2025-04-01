// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;
using CollectionPage = Ssg.Extensions.Metadata.Abstractions.CollectionPage;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    [LdJsonTarget(typeof(CollectionPage))]
    public class CollectionPageLdJsonRenderer : ILdJsonRenderer
    {
        readonly Dictionary<AuthorId, Person> _Authors;
        readonly Dictionary<OrganizationId, Organization> _Organizations;

        public CollectionPageLdJsonRenderer(Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            _Authors = authors;
            _Organizations = organizations;
        }

        Thing ILdJsonRenderer.ToLdJson(BasePage page)
        {
            if (page is not CollectionPage collectionPage)
            {
                throw new InvalidOperationException();
            }

            if ("blog.html".Equals(collectionPage.Uri, StringComparison.Ordinal))
            {
                Blog blogScheme = collectionPage.ToBlog(_Authors, _Organizations);
                return blogScheme;
            }

            Schema.NET.CollectionPage collectionScheme = collectionPage.ToCollectionPage();
            return collectionScheme;
        }
    }
}