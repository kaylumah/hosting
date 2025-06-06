﻿// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public class ParsedFile<T>
    {
        public T FrontMatter
        { get; set; }
        public string Content
        { get; set; }

        public ParsedFile(string content, T data)
        {
            Content = content;
            FrontMatter = data;
        }
    }
}
