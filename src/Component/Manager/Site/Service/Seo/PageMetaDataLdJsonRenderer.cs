// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    [LdJsonTarget(typeof(PageMetaData))]
    public class PageMetaDataLdJsonRenderer : ILdJsonRenderer
    {
        readonly Dictionary<AuthorId, Person> _Authors;
        readonly Dictionary<OrganizationId, Organization> _Organizations;

        public PageMetaDataLdJsonRenderer(Dictionary<AuthorId, Person> authors, Dictionary<OrganizationId, Organization> organizations)
        {
            _Authors = authors;
            _Organizations = organizations;
        }

        Thing ILdJsonRenderer.ToLdJson(BasePage page)
        {
            if (page is not PageMetaData pageMetaData)
            {
                throw new InvalidOperationException();
            }

            WebSite scheme = new WebSite();
            scheme.Name = pageMetaData.Title;
            // scheme.Url = new Uri(renderData.Site.Url);
            WebPage webPageScheme = new WebPage();
            webPageScheme.Name = pageMetaData.Title;
            webPageScheme.Url = page.CanonicalUri;
            webPageScheme.Description = pageMetaData.Description;
            webPageScheme.IsPartOf = scheme;
            return webPageScheme;
        }
    }
}