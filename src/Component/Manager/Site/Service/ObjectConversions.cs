// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service
{
    public class ObjectConversions
    {
        public static AuthorId AuthorId(string author)
        {
            return author;
        }

        public static IEnumerable<Article> ArticlesForTag(SiteMetaData site)
        {
            ArgumentNullException.ThrowIfNull(site);
            IEnumerable<Article> result = Enumerable.Empty<Article>();
            return result;
        }
    }
}