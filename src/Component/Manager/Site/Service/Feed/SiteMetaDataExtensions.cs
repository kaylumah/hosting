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
            Dictionary<string, SyndicationCategory> result;
            if (source.TagMetaData == null)
            {
                result = new();
            }
            else {
                result = source.TagMetaData
                    .ToDictionary(x => x.Id, x => new SyndicationCategory(x.Name));
            }

            return result;
        }

        public static Dictionary<AuthorId, SyndicationPerson> ToPersons(this SiteMetaData source)
        {
            Dictionary<AuthorId, SyndicationPerson> result;

            if (source.AuthorMetaData == null)
            {
                result = new();
            }
            else
            {
                result = source.AuthorMetaData
                    .ToDictionary(x => x.Id, x => {
                        SyndicationPerson syndicationPerson = new SyndicationPerson();
                        syndicationPerson.Name = x.FullName;
                        syndicationPerson.Email = x.Email;
                        syndicationPerson.Uri = GlobalFunctions.AbsoluteUrl(x.Uri);
                        return syndicationPerson;
                    });
            }

            return result;
        }
    }
}
