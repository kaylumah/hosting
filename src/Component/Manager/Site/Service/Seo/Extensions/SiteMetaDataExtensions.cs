// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Utilities;
using Schema.NET;
using Ssg.Extensions.Metadata.Abstractions;

namespace Kaylumah.Ssg.Manager.Site.Service.Seo
{
    public static class SiteMetaDataExtensions
    {
        public static Dictionary<AuthorId, Person> ToPersons(this SiteMetaData source)
        {
            Dictionary<AuthorId, Person> result;

            if (source.AuthorMetaData == null)
            {
                result = [];
            }
            else
            {
                result = source.AuthorMetaData
                    .ToDictionary(x => x.Id, x =>
                    {
                        List<Uri> uris = new List<Uri>();

                        if (!string.IsNullOrEmpty(x.Links.Linkedin))
                        {
                            Uri linkedinUri = new Uri(x.Links.LinkedinProfileUrl!);
                            uris.Add(linkedinUri);
                        }

                        if (!string.IsNullOrEmpty(x.Links.Twitter))
                        {
                            Uri twitterUri = new Uri(x.Links.TwitterProfileUrl!);
                            uris.Add(twitterUri);
                        }

                        Person person = new Person();
                        person.Name = x.FullName;
                        person.Email = x.Email;
                        person.SameAs = new OneOrMany<Uri>(uris);

                        if (!string.IsNullOrEmpty(x.Uri))
                        {
                            Uri personUri = AbsoluteUri(x.Uri);
                            person.Url = personUri;
                        }

                        if (!string.IsNullOrEmpty(x.Picture))
                        {
                            Uri image = AbsoluteUri(x.Picture);
                            person.Image = new Values<IImageObject, Uri>(image);
                        }

                        return person;
                    });
            }

            return result;
        }

        static Uri AbsoluteUri(string url)
        {
            Uri absolute = RenderHelperFunctions.AbsoluteUri(url);
            return absolute;
        }

        public static Dictionary<OrganizationId, Organization> ToOrganizations(this SiteMetaData source)
        {
            Dictionary<OrganizationId, Organization> result;
            if (source.OrganizationMetaData == null)
            {
                result = [];
            }
            else
            {
                result = source.OrganizationMetaData
                    .ToDictionary(x => x.Id, x =>
                    {

                        List<Uri> uris = new List<Uri>
                        {
                            new Uri($"https://www.linkedin.com/company/{x.Linkedin}")
                        };

                        Organization organization = new Organization();
                        organization.Name = x.FullName;
#pragma warning disable RS0030 // not time based
                        organization.FoundingDate = x.Founded.Date;
#pragma warning restore RS0030
                        organization.SameAs = new OneOrMany<Uri>(uris);

                        if (!string.IsNullOrEmpty(x.Logo))
                        {
                            Uri logoUri = AbsoluteUri(x.Logo);
                            organization.Logo =
                                new Values<IImageObject, Uri>(logoUri);
                        }

                        return organization;
                    });
            }

            return result;

        }
    }
}
