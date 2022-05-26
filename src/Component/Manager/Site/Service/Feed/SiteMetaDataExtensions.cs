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
            Dictionary<string, SyndicationPerson> result = new();
            if (source.Data.TryGetValue("authors", out var authorData))
            {
                if (authorData is Dictionary<object, object> authors)
                {
                    foreach (var author in authors)
                    {
                        var singleDictionary = (Dictionary<object, object>)author.Value;
                        var syndicationPerson = new SyndicationPerson
                        {
                            Name = (string)singleDictionary["full_name"],
                            Email = (string)singleDictionary["email"],
                            Uri = (string)singleDictionary["uri"]
                        };
                        result.Add((string)author.Key, syndicationPerson);
                    }
                }
            }
            return result;
        }
    }
}
