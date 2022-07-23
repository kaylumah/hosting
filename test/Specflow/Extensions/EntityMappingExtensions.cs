// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

// ReSharper disable once CheckNamespace

using Kaylumah.Ssg.Engine.Transformation.Interface;

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
            Id = author.Ids
        };
    }
}
