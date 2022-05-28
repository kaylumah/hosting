// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Schema.NET;

namespace Kaylumah.Ssg.Manager.Site.Service.StructureData;

public static class SiteMetaDataExtensions
{
    public static Dictionary<string, Person> ToPersons(this SiteMetaData source)
    {
        Dictionary<string, Person> result = new();
        if (source.Data.TryGetValue("authors", out var authorData))
        {
            if (authorData is Dictionary<object, object> authors)
            {
                foreach (var author in authors)
                {
                    var singleDictionary = (Dictionary<object, object>)author.Value;
                    var syndicationPerson = new Person
                    {
                        Name = (string)singleDictionary["full_name"],
                        // Email = (string)singleDictionary["email"],
                        // Uri = new Uri((string)singleDictionary["uri"])
                    };
                    result.Add((string)author.Key, syndicationPerson);
                }
            }
        }
        return result;
    }
}
