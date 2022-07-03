// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.ServiceModel.Syndication;
using Kaylumah.Ssg.Engine.Transformation.Interface;

namespace Kaylumah.Ssg.Manager.Site.Service.Feed
{
    public static class SiteMetaDataExtensions
    {
        public static Dictionary<string, SyndicationCategory> ToCategories(this SiteMetaData source)
        {
            var tags = source.TagMetaData
                    .ToDictionary(x => x.Id, x => new SyndicationCategory(x.Name));
            return tags;
        }

        public static Dictionary<string, SyndicationPerson> ToPersons(this SiteMetaData source)
        {
            var authors = source.AuthorMetaData
                    .ToDictionary(x => x.Id, x => new SyndicationPerson()
                    {
                        Name = x.FullName,
                        Email = x.Email,
                        Uri = x.Uri
                    });
            return authors;
        }
    }
}
