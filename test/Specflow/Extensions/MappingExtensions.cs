// Copyright (c) Kaylumah, 2022. All rights reserved.
// See LICENSE file in the project root for full license information.

using Kaylumah.Ssg.Engine.Transformation.Interface;
using Test.Specflow.Entities;

namespace Test.Specflow.Extensions;

public static partial class MappingExtensions
{
    public static IEnumerable<Article> ToArticles(this IEnumerable<PageMetaData> pageMetaData)
    {
        return pageMetaData.Select(x => new Article()
        {
            Uri = x.Uri,
            Title = x.Title,
            Description = x.Description,
            Created = x.Published != DateTimeOffset.MinValue ? x.Published : null,
            Modified = x.Modified != DateTimeOffset.MinValue ? x.Modified : null
        });
    }
}
