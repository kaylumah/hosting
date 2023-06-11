// Copyright (c) Kaylumah, 2023. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Kaylumah.Ssg.Engine.Transformation.Interface;

// ReSharper disable once CheckNamespace
namespace Test.Specflow.Entities;

public static class EntityMappingExtensions
{
    public static IEnumerable<AuthorMetaData> ToAuthorMetadata(this IEnumerable<Author> authors)
    {
        return authors.Select(ToAuthorMetadata);
    }

    public static AuthorMetaData ToAuthorMetadata(this Author author)
    {
        return new AuthorMetaData()
        {
            Id = author.Id,
            FullName = author.Name,
            Email = author.Email,
            Uri = author.Uri,
            Picture = author.Picture,
            Links = new Links()
            {
                Devto = author.Id,
                Github = author.Id,
                Linkedin = author.Id,
                Medium = author.Id,
                Twitter = author.Id
            }
        };
    }

    public static IEnumerable<OrganizationMetaData> ToOrganizationMetadata(this IEnumerable<Organization> organizations)
    {
        return organizations.Select(ToOrganizationMetadata);
    }

    public static OrganizationMetaData ToOrganizationMetadata(this Organization organization)
    {
        return new OrganizationMetaData()
        {
            Id = organization.Id,
            FullName = organization.Name
        };
    }

    public static IEnumerable<TagMetaData> ToTagMetadata(this IEnumerable<Tag> tags)
    {
        return tags.Select(ToTagMetadata);
    }

    public static TagMetaData ToTagMetadata(this Tag tag)
    {
        return new TagMetaData()
        {
            Id = tag.Id,
            Name = tag.Id
        };
    }
}
