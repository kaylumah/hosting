// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Schema.NET;

namespace Kaylumah.Ssg.Manager.Site.Service.StructureData;

public static class SiteMetaDataExtensions
{
    public static Dictionary<string, Person> ToPersons(this SiteMetaData source)
    {
        var authors = source.AuthorMetaData
            .ToDictionary(x => x.Id, x => new Person() {
                Name = x.FullName,
                Url = new Uri(x.Uri)
            });
        return authors;
    }
}
