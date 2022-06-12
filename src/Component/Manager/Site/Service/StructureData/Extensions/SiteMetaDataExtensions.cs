// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Kaylumah.Ssg.Utilities;
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

    public static Dictionary<string, Organization> ToOrganizations(this SiteMetaData source)
    {
        # pragma warning disable RS0030
        var organizations = source.OrganizationMetaData
            .ToDictionary(x => x.Id, x => {

                var uris = new List<Uri>
                {
                    new Uri($"https://www.linkedin.com/company/{x.Linkedin}")
                };

                var organization = new Organization() {
                    Logo = new Values<IImageObject, Uri>(new Uri(GlobalFunctions.AbsoluteUrl((string)x.Logo))),
                    FoundingDate = x.Founded.Date,
                    SameAs = new OneOrMany<Uri>(uris)
                };
                return organization;
            });
        return organizations;
        # pragma warning restore RS0030
    }
}
