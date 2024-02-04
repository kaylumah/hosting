// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Ssg.Extensions.Metadata.Abstractions;

// ReSharper disable once CheckNamespace
namespace Test.Unit.Entities
{
    public static class EntityMappingExtensions
    {
        public static IEnumerable<AuthorMetaData> ToAuthorMetadata(this IEnumerable<Author> authors)
        {
            return authors.Select(ToAuthorMetadata);
        }

        public static AuthorMetaData ToAuthorMetadata(this Author author)
        {
            AuthorMetaData result = new AuthorMetaData(author.Id, author.Name, author.Email, author.Uri, author.Picture);
            result.Links.Devto = author.Id;
            result.Links.Github = author.Id;
            result.Links.Linkedin = author.Id;
            result.Links.Medium = author.Id;
            result.Links.Twitter = author.Id;
            return result;
        }

        public static IEnumerable<OrganizationMetaData> ToOrganizationMetadata(this IEnumerable<Organization> organizations)
        {
            return organizations.Select(ToOrganizationMetadata);
        }

        public static OrganizationMetaData ToOrganizationMetadata(this Organization organization)
        {
#pragma warning disable RS0030 // Do not use banned APIs
            return new OrganizationMetaData(organization.Id, organization.Name, null, null, null, System.DateTimeOffset.Now);
#pragma warning restore RS0030 // Do not use banned APIs
        }

        public static IEnumerable<TagMetaData> ToTagMetadata(this IEnumerable<Tag> tags)
        {
            return tags.Select(ToTagMetadata);
        }

        public static TagMetaData ToTagMetadata(this Tag tag)
        {
            return new TagMetaData(tag.Id, tag.Id, null, null);
        }
    }
}
