// Copyright (c) Kaylumah, 2025. All rights reserved.
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
            IEnumerable<AuthorMetaData> result = authors.Select(ToAuthorMetadata);
            return result;
        }

        public static AuthorMetaData ToAuthorMetadata(this Author author)
        {
            Links links = new Links();
            links.Devto = author.Id;
            links.Github = author.Id;
            links.Linkedin = author.Id;
            links.Medium = author.Id;
            links.Twitter = author.Id;
            AuthorMetaData result = new AuthorMetaData();
            result.Id = author.Id!;
            result.FullName = author.Name!;
            result.Email = author.Email!;
            result.Uri = author.Uri!;
            result.Picture = author.Picture!;
            result.Links = links;
            return result;
        }

        public static IEnumerable<OrganizationMetaData> ToOrganizationMetadata(this IEnumerable<Organization> organizations)
        {
            IEnumerable<OrganizationMetaData> result = organizations.Select(ToOrganizationMetadata);
            return result;
        }

        public static OrganizationMetaData ToOrganizationMetadata(this Organization organization)
        {
            OrganizationMetaData result = new OrganizationMetaData();
            result.Id = organization.Id!;
            result.FullName = organization.Name!;
            return result;
        }

        public static IEnumerable<TagMetaData> ToTagMetadata(this IEnumerable<Tag> tags)
        {
            IEnumerable<TagMetaData> result = tags.Select(ToTagMetadata);
            return result;
        }

        public static TagMetaData ToTagMetadata(this Tag tag)
        {
            TagMetaData result = new TagMetaData();
            result.Id = tag.Id!;
            result.Name = tag.Id!;
            return result;
        }
    }
}
