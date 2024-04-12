// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Threading;

namespace Ssg.Extensions.Metadata.Abstractions
{
    public class RenderHelperFunctions
    {
        public static AsyncLocal<string> Url
        { get; } = new();

        public static Uri AbsoluteUri(string source)
        {
            string baseUrl = Url.Value!;
            Uri result;

            if (source.StartsWith('/'))
            {
                source = source[1..];
            }

            result = new Uri($"{baseUrl}/{source}");

            return result;
        }
    }
}