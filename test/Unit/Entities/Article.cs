﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;

namespace Test.Unit.Entities
{
    public class Article
    {
        public string? Uri
        { get; set; }
        public string? Title
        { get; set; }
        public string? Description
        { get; set; }
        public DateTimeOffset? Created
        { get; set; }
        public DateTimeOffset? Modified
        { get; set; }
        public string[] Tags
        { get; set; } = Array.Empty<string>();
        public string? Author
        { get; set; }
    }

    public class ArticleCollection : MockEntityCollection<string, Article>
    {
        public override string BuildKey(Article item)
        {
            return item.Uri ?? string.Empty;
        }
    }
}
