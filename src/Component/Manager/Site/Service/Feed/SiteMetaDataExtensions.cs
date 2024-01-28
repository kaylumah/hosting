// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using Kaylumah.Ssg.Utilities;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Feed
{
    public static class SiteMetaDataExtensions
    {
        public static Dictionary<string, SyndicationCategory> ToCategories(this SiteMetaData source)
        {
            if (source.TagMetaData == null)
            {
                return new();
            }

            Dictionary<string, SyndicationCategory> tags = source.TagMetaData
                    .ToDictionary(x => x.Id, x => new SyndicationCategory(x.Name));
            return tags;
        }

        public static Dictionary<AuthorId, SyndicationPerson> ToPersons(this SiteMetaData source)
        {
            if (source.AuthorMetaData == null)
            {
                return new();
            }

            Dictionary<AuthorId, SyndicationPerson> authors = source.AuthorMetaData
                    .ToDictionary(x => x.Id, x => new SyndicationPerson()
                    {
                        Name = x.FullName,
                        Email = x.Email,
                        Uri = GlobalFunctions.AbsoluteUrl(x.Uri)
                    });
            return authors;
        }
    }
}
