// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

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
            Id = author.Id
        };
    }
}
