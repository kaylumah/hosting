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
            AuthorMetaData result = new AuthorMetaData();
            result.Id = author.Id;
            result.FullName = author.Name;
            result.Email = author.Email;
            result.Uri = author.Uri;
            result.Picture = author.Picture;
            result.Links = new Links()
            {
                Devto = author.Id,
                Github = author.Id,
                Linkedin = author.Id,
                Medium = author.Id,
                Twitter = author.Id
            };
            return result;
        }

        public static IEnumerable<OrganizationMetaData> ToOrganizationMetadata(this IEnumerable<Organization> organizations)
        {
            return organizations.Select(ToOrganizationMetadata);
        }

        public static OrganizationMetaData ToOrganizationMetadata(this Organization organization)
        {
            OrganizationMetaData result = new OrganizationMetaData();
            result.Id = organization.Id;
            result.FullName = organization.Name;
            return result;
        }

        public static IEnumerable<TagMetaData> ToTagMetadata(this IEnumerable<Tag> tags)
        {
            return tags.Select(ToTagMetadata);
        }

        public static TagMetaData ToTagMetadata(this Tag tag)
        {
            TagMetaData result = new TagMetaData();
            result.Id = tag.Id;
            result.Name = tag.Id;
            return result;
        }
    }
}
