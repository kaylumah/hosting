// Copyright (c) Kaylumah, 2024. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Text.Json;
using Ssg.Extensions.Data.Abstractions;

namespace Ssg.Extensions.Data.Json
{
    public class JsonParser : IJsonParser
    {
        T IParser.Parse<T>(string raw)
        {
            // JsonNode? result = JsonSerializer.Deserialize<JsonNode>(raw);
            T? result = JsonSerializer.Deserialize<T>(raw);
            Debug.Assert(result != null);
            return result;
        }
    }
}