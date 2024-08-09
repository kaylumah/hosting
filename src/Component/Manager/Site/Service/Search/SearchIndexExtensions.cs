// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.IO;
using System.Text.Json;

namespace Kaylumah.Ssg.Manager.Site.Service.Search
{
    public static class SearchIndexExtensions
    {
        public static byte[] SaveAsJson(this SearchIndex searchIndex)
        {
#pragma warning disable CA1869
            JsonSerializerOptions options = new JsonSerializerOptions();
            using MemoryStream stream = new MemoryStream();
            JsonSerializer.Serialize(stream, searchIndex, options);

            byte[] result = stream.ToArray();
            return result;
        }
    }
}
