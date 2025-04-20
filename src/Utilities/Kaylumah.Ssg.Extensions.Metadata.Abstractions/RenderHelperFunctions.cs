// Copyright (c) Kaylumah, 2025. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using System.Threading;

namespace Kaylumah.Ssg.Extensions.Metadata.Abstractions
{
    public class RenderHelperFunctions
    {
        public static Uri AbsoluteUri(string baseUrl, string source)
        {
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