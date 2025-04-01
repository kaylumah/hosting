// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    [LdJsonTarget(typeof(ArticleMetaData))]
    public class ArticleMetaDataLdJsonRenderer : ILdJsonRenderer
    {
        readonly Dictionary<AuthorId, Person> _Authors;
        readonly Dictionary<OrganizationId, Organization> _Organizations;

        public ArticleMetaDataLdJsonRenderer(Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            _Authors = authors;
            _Organizations = organizations;
        }

        Thing ILdJsonRenderer.ToLdJson(BasePage page)
        {
            if (page is not ArticleMetaData article)
            {
                throw new InvalidOperationException();
            }

            BlogPosting blogPostScheme = article.ToBlogPosting(_Authors, _Organizations);
            return blogPostScheme;
        }
    }
}