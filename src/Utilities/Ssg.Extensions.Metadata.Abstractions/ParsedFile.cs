// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class ParsedFile<T>
    {
        public T Data
        { get; set; }
        public string Content
        { get; set; }

        public ParsedFile(string content, T data)
        {
            Content = content;
            Data = data;
        }
    }
}
